import { describe, expect, it } from "vitest";
import { safeRegistrationCause, safeSdkErrorCode } from "./registration-diagnostics";

describe("registration diagnostics", () => {
  it("keeps only bounded SDK error codes", () => {
    expect(safeSdkErrorCode({ code: "auth/insufficient-permission" })).toBe("auth/insufficient-permission");
    expect(safeSdkErrorCode({ code: "bad code containing private text" })).toBe("UNSAFE_ERROR_CODE");
  });

  it("classifies signBlob failures without returning the original message", () => {
    const error = Object.assign(new Error("Permission denied for iam.serviceAccounts.signBlob on a private resource"), {
      code: "auth/insufficient-permission",
    });
    expect(safeRegistrationCause(error)).toBe("SIGN_BLOB_PERMISSION_DENIED");
  });

  it("uses bounded categories for other SDK failures", () => {
    expect(safeRegistrationCause({ code: "database/permission-denied" })).toBe("SDK_PERMISSION_DENIED");
    expect(safeRegistrationCause({ code: "auth/internal-error" })).toBe("SDK_INTERNAL_ERROR");
    expect(safeRegistrationCause(new Error("private and unclassified details"))).toBe("UNCLASSIFIED_SDK_ERROR");
  });
});
