using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable] public class SendCodeRequest { public string email; public string previousSessionId; }
[Serializable] public class SendCodeResponse { public string sessionId; public int expiresInSeconds; public int resendAfterSeconds; }
[Serializable] public class VerifyCodeRequest { public string email; public string sessionId; public string code; }
[Serializable] public class VerifyCodeResponse { public string signupGrant; public int expiresInSeconds; }
[Serializable] public class AvailabilityRequest { public string kind; public string value; }
[Serializable] public class AvailabilityResponse { public bool available; }
[Serializable] public class CompleteRegistrationRequest { public string email; public string sessionId; public string signupGrant; public string nickname; public string phone; public string password; }
[Serializable] public class CustomTokenResponse { public string customToken; }
[Serializable] public class AccountApiError { public string error; public string message; public int retryAfterSeconds; }

public sealed class AccountServerException : Exception
{
    public string Code { get; private set; }
    public int RetryAfterSeconds { get; private set; }
    public AccountServerException(string code, string message, int retryAfterSeconds) : base(message)
    { Code = code; RetryAfterSeconds = retryAfterSeconds; }
}

public sealed class AccountServerClient
{
    readonly string baseUrl;
    public AccountServerClient(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Account API base URL is required.", nameof(baseUrl));
        this.baseUrl = baseUrl.TrimEnd('/');
    }

    public Task<SendCodeResponse> SendVerificationCodeAsync(string email, string previousSessionId) =>
        PostAsync<SendCodeRequest, SendCodeResponse>("sendVerificationCode", new SendCodeRequest { email = email, previousSessionId = previousSessionId }, null);
    public Task<VerifyCodeResponse> VerifyCodeAsync(string email, string sessionId, string code) =>
        PostAsync<VerifyCodeRequest, VerifyCodeResponse>("verifyVerificationCode", new VerifyCodeRequest { email = email, sessionId = sessionId, code = code }, null);
    public Task<AvailabilityResponse> CheckAvailabilityAsync(string kind, string value) =>
        PostAsync<AvailabilityRequest, AvailabilityResponse>("checkRegistrationAvailability", new AvailabilityRequest { kind = kind, value = value }, null);
    public Task<CustomTokenResponse> CompleteRegistrationAsync(CompleteRegistrationRequest request) =>
        PostAsync<CompleteRegistrationRequest, CustomTokenResponse>("completeRegistration", request, null);

    async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest bodyObject, string idToken)
    {
        byte[] body = Encoding.UTF8.GetBytes(JsonUtility.ToJson(bodyObject));
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/" + endpoint, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 30;
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(idToken)) request.SetRequestHeader("Authorization", "Bearer " + idToken);
            await SendAsync(request);
            string responseText = request.downloadHandler.text;
            if (request.result != UnityWebRequest.Result.Success)
            {
                AccountApiError error = null;
                try { error = JsonUtility.FromJson<AccountApiError>(responseText); } catch { }
                string errorCode = SafeDiagnosticCode(error != null ? error.error : "NETWORK_ERROR");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"Account API failure function={endpoint} http={request.responseCode} code={errorCode}");
#endif
                throw new AccountServerException(errorCode,
                    error != null && !string.IsNullOrEmpty(error.message) ? error.message : "계정 서버 요청에 실패했습니다.",
                    error != null ? error.retryAfterSeconds : 0);
            }
            return JsonUtility.FromJson<TResponse>(responseText);
        }
    }

    static string SafeDiagnosticCode(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length > 64) return "UNSAFE_ERROR_CODE";
        for (int i = 0; i < value.Length; i++)
        {
            char character = value[i];
            if (!((character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9') || character == '_'))
                return "UNSAFE_ERROR_CODE";
        }
        return value;
    }

    static Task SendAsync(UnityWebRequest request)
    {
        TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        operation.completed += _ => completion.TrySetResult(true);
        return completion.Task;
    }
}
