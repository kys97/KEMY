import { describe, expect, it } from "vitest";
import { hmac, nicknameKey, normalizeEmail, normalizeNickname, normalizePhone, registrationUid, safeEqualHex, validNickname, validPassword } from "./security";

describe("account security helpers", () => {
  it("normalizes email and Unicode nicknames consistently", () => {
    expect(normalizeEmail(" User+tag@Sub.Example.COM ")).toBe("user+tag@sub.example.com");
    expect(normalizeNickname("  Ｋｅｍｙ  ")).toBe("Kemy");
    expect(nicknameKey("KEMY")).toBe(nicknameKey("kemy"));
    expect(validNickname("한글닉네임")).toBe(true);
    expect(validNickname("bad\nname")).toBe(false);
    expect(validNickname("123456789")).toBe(false);
  });
  it("normalizes only allowed Korean mobile formats", () => {
    expect(normalizePhone("01012345678")).toBe("01012345678");
    expect(normalizePhone("010-1234-5678")).toBe("01012345678");
    expect(normalizePhone("010 1234 5678")).toBe("01012345678");
    expect(normalizePhone("\t01012345678")).toBeNull();
    expect(normalizePhone("010\u00a01234\u00a05678")).toBeNull();
    expect(normalizePhone("abc01012345678")).toBeNull();
  });
  it("enforces the shared password policy", () => {
    expect(validPassword("password1")).toBe(true);
    expect(validPassword("Password1!")).toBe(true);
    expect(validPassword("password")).toBe(false);
    expect(validPassword("12345678")).toBe(false);
    expect(validPassword("password 1")).toBe(false);
    expect(validPassword("비밀번호123")).toBe(false);
  });
  it("compares verification hashes and derives stable uids", () => {
    const digest = hmac("secret", "signup", "a@example.com", "session", "012345", 123);
    expect(safeEqualHex(digest, digest)).toBe(true);
    expect(registrationUid("secret", "session", "a@example.com")).toBe(registrationUid("secret", "session", "a@example.com"));
  });
});
