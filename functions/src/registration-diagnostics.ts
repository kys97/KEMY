export type RegistrationStage =
  | "REQUEST_VALIDATION"
  | "GRANT_VALIDATION"
  | "INDEX_RESERVATION"
  | "AUTH_ACCOUNT"
  | "PROFILE_WRITE"
  | "CUSTOM_CLAIMS"
  | "CUSTOM_TOKEN";

export function safeSdkErrorCode(error: unknown): string {
  const value = String((error as { code?: unknown } | null)?.code ?? "UNKNOWN");
  return /^[A-Za-z0-9_./-]{1,80}$/.test(value) ? value : "UNSAFE_ERROR_CODE";
}

export function safeRegistrationCause(error: unknown): string {
  const code = safeSdkErrorCode(error).toLowerCase();
  const message = error instanceof Error ? error.message.toLowerCase() : "";
  if (message.includes("iam.serviceaccounts.signblob") &&
      (message.includes("permission") || message.includes("denied") || message.includes("unauthorized"))) {
    return "SIGN_BLOB_PERMISSION_DENIED";
  }
  if (code.includes("permission-denied") || code.includes("insufficient-permission") || code === "permission_denied") {
    return "SDK_PERMISSION_DENIED";
  }
  if (code.includes("network") || code.includes("unavailable") || code.includes("timeout")) return "SDK_UNAVAILABLE";
  if (code.includes("internal")) return "SDK_INTERNAL_ERROR";
  if (error instanceof Error && error.name === "ApiError") return "EXPECTED_API_REJECTION";
  return "UNCLASSIFIED_SDK_ERROR";
}
