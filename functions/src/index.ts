import { randomInt, randomUUID } from "node:crypto";
import { initializeApp } from "firebase-admin/app";
import { getAppCheck } from "firebase-admin/app-check";
import { getAuth } from "firebase-admin/auth";
import { getDatabase } from "firebase-admin/database";
import { defineSecret, defineString } from "firebase-functions/params";
import { onRequest } from "firebase-functions/v2/https";
import { error as logError, info as logInfo } from "firebase-functions/logger";
import { Resend } from "resend";
import { RegistrationStage, safeRegistrationCause, safeSdkErrorCode } from "./registration-diagnostics";
import { hmac, nicknameKey, normalizeEmail, normalizeNickname, normalizePhone, registrationUid, safeEqualHex, validEmail, validNickname, validPassword } from "./security";
import { VerificationSession, verifySessionTransaction } from "./verification";

initializeApp();

const resendApiKey = defineSecret("RESEND_API_KEY");
const codeHmacSecret = defineSecret("CODE_HMAC_SECRET");
const resendFromEmail = defineString("RESEND_FROM_EMAIL");
const requireAppCheck = defineString("REQUIRE_APP_CHECK", { default: "false" });
const region = "asia-northeast3";
const codeLifetimeMs = 5 * 60 * 1000;
const resendDelayMs = 60 * 1000;
const grantLifetimeMs = 10 * 60 * 1000;
const secrets = [resendApiKey, codeHmacSecret];
const verifyCodeBuildId = "verify-code-20261017-cold-cache-v3";
const completeRegistrationBuildId = "complete-registration-20261017-diagnostics-v1";

type Json = Record<string, unknown>;
class ApiError extends Error {
  constructor(public status: number, public code: string, message: string, public retryAfterSeconds = 0) { super(message); }
}

const handlerOptions = { region, secrets, timeoutSeconds: 30, memory: "256MiB" as const, maxInstances: 20 };

function ok(res: import("express").Response, body: Json): void { res.status(200).json(body); }
function fail(res: import("express").Response, error: unknown): void {
  const api = error instanceof ApiError ? error : new ApiError(500, "INTERNAL", "요청을 처리하지 못했습니다.");
  res.status(api.status).json({ error: api.code, message: api.message, retryAfterSeconds: api.retryAfterSeconds });
}
function assertPost(req: import("express").Request): void {
  if (req.method !== "POST") throw new ApiError(405, "METHOD_NOT_ALLOWED", "POST 요청만 허용됩니다.");
  if (!req.is("application/json")) throw new ApiError(415, "JSON_REQUIRED", "JSON 요청이 필요합니다.");
}
async function assertAppCheck(req: import("express").Request): Promise<void> {
  if (requireAppCheck.value() !== "true") return;
  const token = req.header("X-Firebase-AppCheck");
  if (!token) throw new ApiError(401, "APP_CHECK_REQUIRED", "앱 확인에 실패했습니다.");
  try { await getAppCheck().verifyToken(token); } catch { throw new ApiError(401, "APP_CHECK_INVALID", "앱 확인에 실패했습니다."); }
}
function ipAddress(req: import("express").Request): string { return req.ip || req.socket.remoteAddress || "unknown"; }
function keyFor(secret: string, kind: string, value: string): string { return hmac(secret, "rate", kind, value).slice(0, 40); }

async function enforceRateLimit(key: string, limit: number, windowMs: number): Promise<void> {
  const ref = getDatabase().ref(`serverOnly/rateLimits/${key}`);
  const now = Date.now();
  let blocked = false;
  let retryAfter = 0;
  await ref.transaction((value) => {
    const current = value as { windowStart?: number; count?: number } | null;
    if (!current?.windowStart || now - current.windowStart >= windowMs) return { windowStart: now, count: 1 };
    if ((current.count ?? 0) >= limit) {
      blocked = true; retryAfter = Math.max(1, Math.ceil((current.windowStart + windowMs - now) / 1000)); return;
    }
    return { windowStart: current.windowStart, count: (current.count ?? 0) + 1 };
  }, undefined, false);
  if (blocked) throw new ApiError(429, "RATE_LIMITED", "요청이 너무 많습니다. 잠시 후 다시 시도해 주세요.", retryAfter);
}

function nicknameIndexKey(secret: string, nickname: string): string {
  return hmac(secret, "nickname", nicknameKey(nickname));
}
function phoneIndexKey(secret: string, phone: string): string {
  return hmac(secret, "phone", phone);
}
async function legacyNicknameExists(canonical: string, excludedUid = ""): Promise<boolean> {
  const snapshot = await getDatabase().ref("UserInfo").get();
  let found = false;
  snapshot.forEach((child) => {
    if (child.key === excludedUid) return;
    const name = child.child("name").val();
    if (typeof name === "string" && nicknameKey(name) === canonical) found = true;
  });
  return found;
}

export const checkRegistrationAvailability = onRequest(handlerOptions, async (req, res) => {
  try {
    assertPost(req); await assertAppCheck(req);
    const kind = String(req.body?.kind ?? "");
    const secret = codeHmacSecret.value();
    await enforceRateLimit(`availability_${keyFor(secret, "availability-ip", ipAddress(req))}`, 40, 15 * 60 * 1000);
    if (kind === "nickname") {
      const nickname = normalizeNickname(req.body?.value);
      if (!validNickname(nickname)) throw new ApiError(400, "INVALID_NICKNAME", "닉네임은 공백·제어문자 없이 1~8글자로 입력해 주세요.");
      const key = nicknameIndexKey(secret, nickname);
      const indexed = (await getDatabase().ref(`serverOnly/registrationIndexes/nicknames/${key}`).get()).exists();
      const legacy = indexed ? false : await legacyNicknameExists(nicknameKey(nickname));
      ok(res, { available: !indexed && !legacy }); return;
    }
    if (kind === "phone") {
      const phone = normalizePhone(req.body?.value);
      if (!phone) throw new ApiError(400, "INVALID_PHONE", "010 휴대폰 번호를 확인해 주세요.");
      const key = phoneIndexKey(secret, phone);
      const indexed = (await getDatabase().ref(`serverOnly/registrationIndexes/phones/${key}`).get()).exists();
      ok(res, { available: !indexed }); return;
    }
    throw new ApiError(400, "INVALID_AVAILABILITY_KIND", "중복 검사 항목을 확인해 주세요.");
  } catch (error) { fail(res, error); }
});

export const sendVerificationCode = onRequest(handlerOptions, async (req, res) => {
  try {
    assertPost(req); await assertAppCheck(req);
    const email = normalizeEmail(req.body?.email);
    if (!validEmail(email)) throw new ApiError(400, "INVALID_EMAIL", "올바른 이메일을 입력해 주세요.");
    const secret = codeHmacSecret.value();
    const emailKey = keyFor(secret, "email", email);
    const ipKey = keyFor(secret, "ip", ipAddress(req));
    await Promise.all([
      enforceRateLimit(`email_${emailKey}`, 5, 60 * 60 * 1000),
      enforceRateLimit(`ip_${ipKey}`, 20, 60 * 60 * 1000),
      enforceRateLimit("global_send", 300, 60 * 1000),
    ]);

    const now = Date.now();
    const previousSessionId = String(req.body?.previousSessionId ?? "");
    if (previousSessionId) {
      const previousRef = getDatabase().ref(`serverOnly/verificationSessions/${previousSessionId}`);
      const previous = (await previousRef.get()).val() as VerificationSession | null;
      if (previous?.emailKey === emailKey && !previous.revoked && now < previous.resendAvailableAt)
        throw new ApiError(429, "RESEND_TOO_SOON", "60초 후 다시 전송할 수 있습니다.", Math.ceil((previous.resendAvailableAt - now) / 1000));
      if (previous?.emailKey === emailKey) await previousRef.update({ revoked: true });
    }

    const activeRef = getDatabase().ref(`serverOnly/activeVerification/${emailKey}`);
    const oldActive = String((await activeRef.get()).val() ?? "");
    if (oldActive && oldActive !== previousSessionId) {
      const oldRef = getDatabase().ref(`serverOnly/verificationSessions/${oldActive}`);
      const oldSession = (await oldRef.get()).val() as VerificationSession | null;
      if (oldSession?.emailKey === emailKey && !oldSession.revoked && now < oldSession.resendAvailableAt)
        throw new ApiError(429, "RESEND_TOO_SOON", "60초 후 다시 전송할 수 있습니다.", Math.ceil((oldSession.resendAvailableAt - now) / 1000));
      await oldRef.update({ revoked: true });
    }

    const sessionId = randomUUID();
    const code = randomInt(0, 1_000_000).toString().padStart(6, "0");
    const expiresAt = now + codeLifetimeMs;
    let accountExists = false;
    try { await getAuth().getUserByEmail(email); accountExists = true; }
    catch (error: unknown) {
      if ((error as { code?: string }).code !== "auth/user-not-found") throw error;
    }
    const session: VerificationSession = {
      purpose: "signup", email, emailKey,
      codeHash: hmac(secret, "signup", email, sessionId, code, expiresAt),
      expiresAt, resendAvailableAt: now + resendDelayMs, attempts: 0,
      verified: false, revoked: false, createdAt: now, ipKey, accountExists,
    };
    await getDatabase().ref().update({
      [`serverOnly/verificationSessions/${sessionId}`]: session,
      [`serverOnly/activeVerification/${emailKey}`]: sessionId,
    });

    const { error } = await new Resend(resendApiKey.value()).emails.send({
      from: resendFromEmail.value(), to: email,
      subject: accountExists ? "KEMY 로그인 안내" : "KEMY 회원가입 인증코드",
      text: accountExists
        ? "이미 가입된 이메일입니다. KEMY 로그인 또는 비밀번호 재설정을 이용해 주세요."
        : `KEMY 회원가입 인증코드는 ${code}입니다. 5분 안에 입력해 주세요.`,
    });
    if (error) {
      await getDatabase().ref(`serverOnly/verificationSessions/${sessionId}`).update({ revoked: true, deliveryFailed: true });
      throw new ApiError(502, "EMAIL_DELIVERY_FAILED", "인증 메일을 보내지 못했습니다.");
    }
    ok(res, { sessionId, expiresInSeconds: 300, resendAfterSeconds: 60 });
  } catch (error) { fail(res, error); }
});

export const verifyVerificationCode = onRequest(handlerOptions, async (req, res) => {
  const traceId = randomUUID();
  let committed = false;
  let diagnostic = "REQUEST_INVALID";
  logInfo("verification-code.enter", { traceId, failure: "PENDING", committed, buildId: verifyCodeBuildId });
  try {
    assertPost(req); await assertAppCheck(req);
    const email = normalizeEmail(req.body?.email);
    const sessionId = String(req.body?.sessionId ?? "");
    const code = String(req.body?.code ?? "");
    if (!validEmail(email) || !/^[0-9]{6}$/.test(code) || !sessionId) throw new ApiError(400, "INVALID_CODE", "인증코드를 확인해 주세요.");
    const ref = getDatabase().ref(`serverOnly/verificationSessions/${sessionId}`);
    const now = Date.now();
    const secret = codeHmacSecret.value();
    const result = await verifySessionTransaction({ ref, email, sessionId, code, secret, now, grantLifetimeMs });
    committed = result.committed;
    diagnostic = result.diagnostic;
    if (result.outcome === "EXPIRED") throw new ApiError(410, "CODE_EXPIRED", "인증코드가 만료되었습니다.");
    if (result.outcome === "LOCKED") throw new ApiError(429, "CODE_LOCKED", "인증코드 입력 횟수를 초과했습니다. 새 코드를 요청해 주세요.");
    if (result.outcome !== "OK" || !result.grant) throw new ApiError(400, "INVALID_CODE", "인증코드가 올바르지 않습니다.");
    logInfo("verification-code.result", { traceId, failure: "NONE", committed, buildId: verifyCodeBuildId });
    ok(res, { signupGrant: result.grant, expiresInSeconds: 600 });
  } catch (error) {
    if (!(error instanceof ApiError)) diagnostic = "INTERNAL";
    logInfo("verification-code.result", { traceId, failure: diagnostic, committed, buildId: verifyCodeBuildId });
    fail(res, error);
  }
});

export const completeRegistration = onRequest(handlerOptions, async (req, res) => {
  const traceId = randomUUID();
  let stage: RegistrationStage = "REQUEST_VALIDATION";
  const stepSucceeded = (): void => {
    logInfo("registration.step", { traceId, stage, buildId: completeRegistrationBuildId, sdkCode: "NONE", cause: "SUCCEEDED" });
  };
  logInfo("registration.enter", { traceId, stage, buildId: completeRegistrationBuildId, sdkCode: "NONE", cause: "STARTED" });
  try {
    assertPost(req); await assertAppCheck(req);
    const email = normalizeEmail(req.body?.email);
    const nickname = normalizeNickname(req.body?.nickname);
    const phone = normalizePhone(req.body?.phone);
    const password = String(req.body?.password ?? "");
    const sessionId = String(req.body?.sessionId ?? "");
    const grant = String(req.body?.signupGrant ?? "");
    if (!validEmail(email) || !validNickname(nickname) || !phone || !validPassword(password) || !sessionId || !grant)
      throw new ApiError(400, "INVALID_SIGNUP", "가입 정보를 확인해 주세요.");
    stepSucceeded();

    stage = "GRANT_VALIDATION";
    const secret = codeHmacSecret.value();
    const sessionRef = getDatabase().ref(`serverOnly/verificationSessions/${sessionId}`);
    const session = (await sessionRef.get()).val() as VerificationSession | null;
    const now = Date.now();
    const expectedGrant = hmac(secret, "grant", email, sessionId, grant);
    if (!session || session.email !== email || !session.verified || session.revoked || !session.grantHash ||
        !safeEqualHex(expectedGrant, session.grantHash) || !session.grantExpiresAt || now > session.grantExpiresAt)
      throw new ApiError(403, "SIGNUP_GRANT_INVALID", "이메일 인증이 만료되었습니다. 다시 인증해 주세요.");

    const activeSession = String((await getDatabase().ref(`serverOnly/activeVerification/${session.emailKey}`).get()).val() ?? "");
    if (activeSession !== sessionId) throw new ApiError(403, "SIGNUP_GRANT_REVOKED", "새로 발급된 인증코드를 사용해 주세요.");
    stepSucceeded();

    stage = "INDEX_RESERVATION";
    const uid = registrationUid(secret, sessionId, email);
    const nicknameHash = nicknameIndexKey(secret, nickname);
    const phoneHash = phoneIndexKey(secret, phone);
    if (await legacyNicknameExists(nicknameKey(nickname), uid))
      throw new ApiError(409, "NICKNAME_TAKEN", "이미 사용 중인 닉네임입니다.");
    const indexesRef = getDatabase().ref("serverOnly/registrationIndexes");
    const conflict: { value: "nickname" | "phone" | "" } = { value: "" };
    await indexesRef.transaction((raw) => {
      const indexes = raw ?? {};
      const nicknameOwner = indexes.nicknames?.[nicknameHash];
      const phoneOwner = indexes.phones?.[phoneHash];
      if (nicknameOwner && nicknameOwner.uid !== uid) { conflict.value = "nickname"; return; }
      if (phoneOwner && phoneOwner.uid !== uid) { conflict.value = "phone"; return; }
      indexes.nicknames = indexes.nicknames ?? {};
      indexes.phones = indexes.phones ?? {};
      indexes.nicknames[nicknameHash] = nicknameOwner ?? { uid, sessionId, state: "pending", reservedAt: now };
      indexes.phones[phoneHash] = phoneOwner ?? { uid, sessionId, state: "pending", reservedAt: now };
      return indexes;
    }, undefined, false);
    if (conflict.value === "nickname") throw new ApiError(409, "NICKNAME_TAKEN", "이미 사용 중인 닉네임입니다.");
    if (conflict.value === "phone") throw new ApiError(409, "PHONE_TAKEN", "이미 등록된 휴대폰 번호입니다.");
    stepSucceeded();
    const releasePendingIndexes = async (): Promise<void> => {
      await indexesRef.transaction((raw) => {
        if (!raw) return raw;
        if (raw.nicknames?.[nicknameHash]?.uid === uid && raw.nicknames[nicknameHash].sessionId === sessionId && raw.nicknames[nicknameHash].state === "pending")
          delete raw.nicknames[nicknameHash];
        if (raw.phones?.[phoneHash]?.uid === uid && raw.phones[phoneHash].sessionId === sessionId && raw.phones[phoneHash].state === "pending")
          delete raw.phones[phoneHash];
        return raw;
      }, undefined, false);
    };

    stage = "AUTH_ACCOUNT";
    let user;
    try {
      user = await getAuth().getUser(uid);
      if (normalizeEmail(user.email) !== email) {
        await releasePendingIndexes();
        throw new ApiError(409, "ACCOUNT_CONFLICT", "계정 생성 충돌이 발생했습니다.");
      }
    } catch (error: unknown) {
      if (error instanceof ApiError) throw error;
      const code = (error as { code?: string }).code;
      if (code !== "auth/user-not-found") throw error;
      try { user = await getAuth().createUser({ uid, email, password, emailVerified: true, displayName: nickname }); }
      catch (createError: unknown) {
        if ((createError as { code?: string }).code === "auth/email-already-exists") {
          await releasePendingIndexes();
          throw new ApiError(409, "EMAIL_IN_USE", "이미 가입된 이메일입니다.");
        }
        throw createError;
      }
    }
    stepSucceeded();

    stage = "PROFILE_WRITE";
    const profileRef = getDatabase().ref(`UserInfo/${uid}`);
    const existingProfile = await profileRef.get();
    if (existingProfile.exists()) {
      const state = (await getDatabase().ref(`serverOnly/accountState/${uid}`).get()).val();
      if (!state || state.registrationSessionId !== sessionId) throw new ApiError(409, "PROFILE_CONFLICT", "기존 사용자 데이터를 덮어쓸 수 없습니다.");
    } else {
      await getDatabase().ref().update({
        [`UserInfo/${uid}`]: { name: nickname, coin: 0, heart: 0, skinSet: "Cat00", faceSet: "Face00", skinList: "Cat00", faceList: "Face00" },
        [`serverOnly/registrationIndexes/nicknames/${nicknameHash}`]: { uid, sessionId, state: "complete", completedAt: now },
        [`serverOnly/registrationIndexes/phones/${phoneHash}`]: { uid, sessionId, state: "complete", completedAt: now },
        [`serverOnly/privateProfiles/${uid}/phone`]: phone,
        [`serverOnly/accountState/${uid}`]: { registrationComplete: true, registrationSessionId: sessionId, completedAt: now },
        [`serverOnly/verificationSessions/${sessionId}/used`]: true,
      });
    }
    stepSucceeded();
    stage = "CUSTOM_CLAIMS";
    await getAuth().setCustomUserClaims(uid, { registrationComplete: true });
    stepSucceeded();
    stage = "CUSTOM_TOKEN";
    const customToken = await getAuth().createCustomToken(uid, { registrationComplete: true });
    stepSucceeded();
    ok(res, { customToken });
  } catch (error) {
    logError("registration.failure", {
      traceId,
      stage,
      buildId: completeRegistrationBuildId,
      sdkCode: error instanceof ApiError ? error.code : safeSdkErrorCode(error),
      cause: error instanceof ApiError ? "EXPECTED_API_REJECTION" : safeRegistrationCause(error),
    });
    fail(res, error);
  }
});
