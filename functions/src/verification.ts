import { randomBytes, randomUUID } from "node:crypto";
import type { DataSnapshot, Reference } from "firebase-admin/database";
import { hmac, safeEqualHex } from "./security";

export type VerificationSession = {
  purpose: "signup";
  email: string;
  emailKey: string;
  codeHash: string;
  expiresAt: number;
  resendAvailableAt: number;
  attempts: number;
  verified: boolean;
  revoked: boolean;
  grantHash?: string;
  grantExpiresAt?: number;
  used?: boolean;
  createdAt: number;
  ipKey: string;
  accountExists?: boolean;
  verificationOperationId?: string;
  verificationResult?: "verified" | "invalid" | "expired" | "locked";
};

export type VerificationOutcome = "OK" | "INVALID" | "EXPIRED" | "LOCKED";
export type VerificationDiagnostic =
  | "NONE"
  | "SESSION_NOT_FOUND"
  | "PURPOSE_MISMATCH"
  | "EMAIL_MISMATCH"
  | "SESSION_REVOKED"
  | "ACCOUNT_ALREADY_EXISTS"
  | "CODE_EXPIRED"
  | "ATTEMPTS_EXCEEDED"
  | "CODE_HASH_MISMATCH"
  | "ALREADY_PROCESSED"
  | "FINAL_STATE_MISMATCH";

export type VerificationTransactionResult = {
  outcome: VerificationOutcome;
  diagnostic: VerificationDiagnostic;
  committed: boolean;
  grant?: string;
};

type VerifyTransactionOptions = {
  ref: Reference;
  email: string;
  sessionId: string;
  code: string;
  secret: string;
  now: number;
  grantLifetimeMs: number;
  operationId?: string;
  grant?: string;
};

function classifyFinalSession(session: VerificationSession | null, email: string, now: number): VerificationDiagnostic {
  if (!session) return "SESSION_NOT_FOUND";
  if (session.purpose !== "signup") return "PURPOSE_MISMATCH";
  if (session.email !== email) return "EMAIL_MISMATCH";
  if (session.accountExists) return "ACCOUNT_ALREADY_EXISTS";
  if (session.verified || !session.codeHash) return "ALREADY_PROCESSED";
  if (session.attempts >= 5) return "ATTEMPTS_EXCEEDED";
  if (session.revoked) return "SESSION_REVOKED";
  if (now > session.expiresAt) return "CODE_EXPIRED";
  return "FINAL_STATE_MISMATCH";
}

/**
 * Verifies and consumes a code atomically. The initial server read is intentional:
 * it distinguishes a missing session from an empty local RTDB transaction cache.
 * All validation and mutation still happens against the transaction's latest value.
 */
export async function verifySessionTransaction(options: VerifyTransactionOptions): Promise<VerificationTransactionResult> {
  const { ref, email, sessionId, code, secret, now, grantLifetimeMs } = options;
  // Keep the value subscription active through transaction(). A one-off get()
  // or once() can detach and let the synchronized cache be pruned immediately,
  // causing the transaction's first callback to see null again.
  let initialListener: ((snapshot: DataSnapshot) => void) | undefined;
  const initialSnapshot = await new Promise<DataSnapshot>((resolve, reject) => {
    initialListener = (snapshot) => resolve(snapshot);
    ref.on("value", initialListener, reject);
  });
  if (!initialSnapshot.exists()) {
    ref.off("value", initialListener);
    return { outcome: "INVALID", diagnostic: "SESSION_NOT_FOUND", committed: false };
  }

  const operationId = options.operationId ?? randomUUID();
  const grant = options.grant ?? randomBytes(32).toString("base64url");
  const expectedGrantHash = hmac(secret, "grant", email, sessionId, grant);

  try {
    const transaction = await ref.transaction((raw) => {
      const session = raw as VerificationSession | null;
      if (!session || session.purpose !== "signup" || session.email !== email || session.accountExists ||
          session.verified || !session.codeHash || session.attempts >= 5 || session.revoked) return;

      if (now > session.expiresAt) {
        return { ...session, revoked: true, verificationOperationId: operationId, verificationResult: "expired" };
      }
      if (session.attempts >= 5) {
        return { ...session, revoked: true, verificationOperationId: operationId, verificationResult: "locked" };
      }

      const candidate = hmac(secret, "signup", email, sessionId, code, session.expiresAt);
      if (!safeEqualHex(candidate, session.codeHash)) {
        const attempts = session.attempts + 1;
        const locked = attempts >= 5;
        return {
          ...session,
          attempts,
          revoked: locked,
          verificationOperationId: operationId,
          verificationResult: locked ? "locked" : "invalid",
        };
      }

      return {
        ...session,
        verified: true,
        codeHash: "",
        grantHash: expectedGrantHash,
        grantExpiresAt: now + grantLifetimeMs,
        verificationOperationId: operationId,
        verificationResult: "verified",
      };
    }, undefined, false);

    const finalSession = transaction.snapshot.val() as VerificationSession | null;
    if (!transaction.committed || !finalSession || finalSession.verificationOperationId !== operationId) {
      const diagnostic = classifyFinalSession(finalSession, email, now);
      return {
        outcome: diagnostic === "CODE_EXPIRED" ? "EXPIRED" : diagnostic === "ATTEMPTS_EXCEEDED" ? "LOCKED" : "INVALID",
        diagnostic,
        committed: transaction.committed,
      };
    }

    if (finalSession.verificationResult === "expired") return { outcome: "EXPIRED", diagnostic: "CODE_EXPIRED", committed: true };
    if (finalSession.verificationResult === "locked") return { outcome: "LOCKED", diagnostic: "ATTEMPTS_EXCEEDED", committed: true };
    if (finalSession.verificationResult === "invalid") return { outcome: "INVALID", diagnostic: "CODE_HASH_MISMATCH", committed: true };
    if (finalSession.verificationResult === "verified" && finalSession.verified && !finalSession.revoked &&
        finalSession.codeHash === "" && finalSession.grantHash === expectedGrantHash) {
      return { outcome: "OK", diagnostic: "NONE", committed: true, grant };
    }
    return { outcome: "INVALID", diagnostic: "FINAL_STATE_MISMATCH", committed: true };
  } finally {
    ref.off("value", initialListener);
  }
}
