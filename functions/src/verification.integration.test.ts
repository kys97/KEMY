import { spawn } from "node:child_process";
import { join } from "node:path";
import { initializeApp, deleteApp, type App } from "firebase-admin/app";
import { getDatabase, type Reference } from "firebase-admin/database";
import { afterAll, beforeAll, describe, expect, it } from "vitest";
import { hmac } from "./security";
import type { VerificationSession } from "./verification";

const enabled = process.env.RUN_DATABASE_EMULATOR_TESTS === "1";
const describeEmulator = enabled ? describe : describe.skip;
const host = process.env.FIREBASE_DATABASE_EMULATOR_HOST ?? "127.0.0.1:9000";
const namespace = "demo-kemy-verification";
const databaseUrl = `http://${host}?ns=${namespace}`;
const workerPath = join(__dirname, "../test/verification-worker.cjs");
const testSecret = "integration-test-secret";
const testEmail = "test@example.invalid";
const grantLifetimeMs = 10 * 60 * 1000;

type WorkerResult = {
  outcome?: "OK" | "INVALID" | "EXPIRED" | "LOCKED";
  diagnostic?: string;
  committed: boolean;
  grantIssued?: boolean;
  firstCallbackWasNull?: boolean;
  existsAfterAbort?: boolean;
  workerError?: boolean;
};

describeEmulator("verification transactions against the RTDB emulator", () => {
  let app: App;
  let root: Reference;
  let sequence = 0;

  beforeAll(async () => {
    app = initializeApp({ databaseURL: databaseUrl }, "verification-test-writer");
    root = getDatabase(app).ref("integrationVerification");
    await root.remove();
  });

  afterAll(async () => {
    if (root) await root.remove();
    if (app) await deleteApp(app);
  });

  function nextSessionId(): string {
    sequence += 1;
    return `session-${sequence}`;
  }

  async function seed(code: string, overrides: Partial<VerificationSession> = {}): Promise<{ sessionId: string; now: number; path: string }> {
    const sessionId = nextSessionId();
    const now = Date.now();
    const expiresAt = overrides.expiresAt ?? now + 5 * 60 * 1000;
    const session: VerificationSession = {
      purpose: "signup",
      email: testEmail,
      emailKey: "email-key",
      codeHash: hmac(testSecret, "signup", testEmail, sessionId, code, expiresAt),
      expiresAt,
      resendAvailableAt: now + 60 * 1000,
      attempts: 0,
      verified: false,
      revoked: false,
      createdAt: now,
      ipKey: "ip-key",
      ...overrides,
    };
    const path = `integrationVerification/${sessionId}`;
    await getDatabase(app).ref(path).set(session);
    return { sessionId, now, path };
  }

  function runWorker(input: Record<string, unknown>): Promise<WorkerResult> {
    return new Promise((resolve, reject) => {
      const child = spawn(process.execPath, [workerPath], {
        cwd: join(__dirname, ".."),
        env: { ...process.env, FIREBASE_DATABASE_EMULATOR_HOST: host },
        stdio: ["pipe", "pipe", "pipe"],
      });
      let stdout = "";
      let stderr = "";
      child.stdout.setEncoding("utf8").on("data", (chunk) => { stdout += chunk; });
      child.stderr.setEncoding("utf8").on("data", (chunk) => { stderr += chunk; });
      child.on("error", reject);
      child.on("close", (exitCode) => {
        if (exitCode !== 0) return reject(new Error(`verification worker failed (${exitCode}): ${stderr}`));
        try { resolve(JSON.parse(stdout) as WorkerResult); }
        catch { reject(new Error("verification worker returned invalid JSON")); }
      });
      child.stdin.end(JSON.stringify(input));
    });
  }

  function verifyInput(sessionId: string, path: string, code: string, now: number, email = testEmail): Record<string, unknown> {
    return { mode: "fixed", databaseUrl, path, email, sessionId, code, secret: testSecret, now, grantLifetimeMs };
  }

  it("reproduces the legacy cold-cache null callback before the stored session is read", async () => {
    const seeded = await seed("123456");
    const result = await runWorker({ mode: "legacy", databaseUrl, path: seeded.path });
    expect(result).toMatchObject({ firstCallbackWasNull: true, committed: false, existsAfterAbort: true });
  });

  it("verifies a stored session on the first cold-cache request and preserves a leading zero", async () => {
    const seeded = await seed("012345");
    const result = await runWorker(verifyInput(seeded.sessionId, seeded.path, "012345", seeded.now));
    expect(result).toMatchObject({ outcome: "OK", diagnostic: "NONE", committed: true, grantIssued: true });
  });

  it("verifies a normal six-digit code on a cold-cache request", async () => {
    const seeded = await seed("123456");
    expect(await runWorker(verifyInput(seeded.sessionId, seeded.path, "123456", seeded.now)))
      .toMatchObject({ outcome: "OK", diagnostic: "NONE", committed: true, grantIssued: true });
  });

  it("rejects wrong, expired, revoked old, and nonexistent sessions", async () => {
    const wrong = await seed("111111");
    expect(await runWorker(verifyInput(wrong.sessionId, wrong.path, "222222", wrong.now)))
      .toMatchObject({ outcome: "INVALID", diagnostic: "CODE_HASH_MISMATCH", committed: true, grantIssued: false });
    expect((await getDatabase(app).ref(wrong.path).get()).child("attempts").val()).toBe(1);

    const emailMismatch = await seed("222222");
    expect(await runWorker(verifyInput(emailMismatch.sessionId, emailMismatch.path, "222222", emailMismatch.now, "other@example.invalid")))
      .toMatchObject({ outcome: "INVALID", diagnostic: "EMAIL_MISMATCH", committed: false, grantIssued: false });

    const expired = await seed("333333", { expiresAt: Date.now() - 1 });
    expect(await runWorker(verifyInput(expired.sessionId, expired.path, "333333", expired.now)))
      .toMatchObject({ outcome: "EXPIRED", diagnostic: "CODE_EXPIRED", committed: true, grantIssued: false });

    const old = await seed("444444", { revoked: true });
    expect(await runWorker(verifyInput(old.sessionId, old.path, "444444", old.now)))
      .toMatchObject({ outcome: "INVALID", diagnostic: "SESSION_REVOKED", committed: false, grantIssued: false });

    const missingId = nextSessionId();
    expect(await runWorker(verifyInput(missingId, `integrationVerification/${missingId}`, "555555", Date.now())))
      .toMatchObject({ outcome: "INVALID", diagnostic: "SESSION_NOT_FOUND", committed: false, grantIssued: false });
  });

  it("issues only one grant for duplicate concurrent verification requests", async () => {
    const seeded = await seed("654321");
    const input = verifyInput(seeded.sessionId, seeded.path, "654321", seeded.now);
    const results = await Promise.all([runWorker(input), runWorker(input)]);
    expect(results.filter((result) => result.outcome === "OK" && result.committed && result.grantIssued)).toHaveLength(1);
    expect(results.filter((result) => result.outcome === "INVALID" && result.diagnostic === "ALREADY_PROCESSED")).toHaveLength(1);
    expect(await runWorker(input)).toMatchObject({ outcome: "INVALID", diagnostic: "ALREADY_PROCESSED", committed: false, grantIssued: false });
  });

  it("keeps the five-attempt lock and one-time consumption behavior", async () => {
    const seeded = await seed("777777");
    for (let attempt = 1; attempt <= 4; attempt += 1) {
      expect(await runWorker(verifyInput(seeded.sessionId, seeded.path, "000000", seeded.now)))
        .toMatchObject({ outcome: "INVALID", committed: true });
    }
    expect(await runWorker(verifyInput(seeded.sessionId, seeded.path, "000000", seeded.now)))
      .toMatchObject({ outcome: "LOCKED", diagnostic: "ATTEMPTS_EXCEEDED", committed: true, grantIssued: false });
    const finalSession = (await getDatabase(app).ref(seeded.path).get()).val() as VerificationSession;
    expect(finalSession).toMatchObject({ attempts: 5, revoked: true, verified: false });
    expect(await runWorker(verifyInput(seeded.sessionId, seeded.path, "777777", seeded.now)))
      .toMatchObject({ outcome: "LOCKED", diagnostic: "ATTEMPTS_EXCEEDED", committed: false, grantIssued: false });
  });
}, 60_000);
