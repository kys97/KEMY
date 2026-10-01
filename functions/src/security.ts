import { createHmac, timingSafeEqual } from "node:crypto";

export const normalizeEmail = (value: unknown): string => String(value ?? "").trim().toLowerCase();
export const validEmail = (value: string): boolean => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) && value.length <= 254;
export const normalizeNickname = (value: unknown): string => String(value ?? "").trim().normalize("NFKC");
export const nicknameKey = (value: unknown): string => normalizeNickname(value).toLocaleLowerCase("und");
export const validNickname = (value: string): boolean => {
  const normalized = normalizeNickname(value);
  return Array.from(normalized).length >= 1 && Array.from(normalized).length <= 8 && !/[\p{Cc}\p{Cf}\r\n]/u.test(normalized);
};

export function normalizePhone(value: unknown): string | null {
  const raw = String(value ?? "");
  if (!/^[0-9 -]+$/.test(raw)) return null;
  const normalized = raw.replace(/[ -]/g, "");
  return /^010[0-9]{8}$/.test(normalized) ? normalized : null;
}

export const validPassword = (value: string): boolean =>
  value.length >= 8 && value.length <= 20 && /^[\x21-\x7E]+$/.test(value) && /[A-Za-z]/.test(value) && /[0-9]/.test(value);

export function hmac(secret: string, ...parts: Array<string | number>): string {
  return createHmac("sha256", secret).update(parts.join("\u001f"), "utf8").digest("hex");
}

export function safeEqualHex(left: string, right: string): boolean {
  if (!/^[a-f0-9]{64}$/.test(left) || !/^[a-f0-9]{64}$/.test(right)) return false;
  return timingSafeEqual(Buffer.from(left, "hex"), Buffer.from(right, "hex"));
}

export function registrationUid(secret: string, sessionId: string, email: string): string {
  return `r_${hmac(secret, "uid", sessionId, email).slice(0, 38)}`;
}
