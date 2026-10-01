import { readFileSync } from "node:fs";
import { resolve } from "node:path";
import { describe, expect, it } from "vitest";

const functionsRoot = resolve(__dirname, "..");
const repositoryRoot = resolve(functionsRoot, "..");

describe("test deployment configuration", () => {
  it("binds only the secrets used by registration functions", () => {
    const source = readFileSync(resolve(__dirname, "index.ts"), "utf8");
    expect(source).toContain("const secrets = [resendApiKey, codeHmacSecret]");
    expect(source).not.toContain("AUTH_WEB_API_KEY");
    expect(source).not.toContain("FIREBASE_WEB_API_KEY");
  });

  it("targets Node 22 and the configured Functions and RTDB sources", () => {
    const packageJson = JSON.parse(readFileSync(resolve(functionsRoot, "package.json"), "utf8"));
    const firebaseJson = JSON.parse(readFileSync(resolve(repositoryRoot, "firebase.json"), "utf8"));
    expect(packageJson.engines.node).toBe("22");
    expect(firebaseJson.functions[0]).toMatchObject({ source: "functions", codebase: "default" });
    expect(firebaseJson.database.rules).toBe("database.rules.json");
  });

  it("provides a non-secret Resend onboarding test configuration", () => {
    const example = readFileSync(resolve(functionsRoot, ".env.test.example"), "utf8");
    const source = readFileSync(resolve(__dirname, "index.ts"), "utf8");
    expect(source).toContain('defineString("RESEND_FROM_EMAIL")');
    expect(example).toContain("RESEND_FROM_EMAIL=onboarding@resend.dev");
    expect(example).toContain("REQUIRE_APP_CHECK=false");
  });
});
