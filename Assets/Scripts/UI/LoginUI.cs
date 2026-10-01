using System;
using System.Net.Mail;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header("Account server")]
    [SerializeField] string accountApiBaseUrl;
    [Header("Login")]
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button loginButton;
    [SerializeField] Button resetPasswordButton;
    [SerializeField] TMP_Text statusText;
    bool busy;

    void Start()
    {
        if (!ValidateReferences()) { enabled = false; return; }
        string baseUrl = accountApiBaseUrl;
        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = "https://asia-northeast3-" + FirebaseApp.DefaultInstance.Options.ProjectId + ".cloudfunctions.net";
        GameManager.Instance.AccountService.ConfigureAccountServer(baseUrl);
        joinButton.onClick.AddListener(OpenJoin);
        loginButton.onClick.AddListener(Login);
        resetPasswordButton.onClick.AddListener(ResetPassword);
        SetStatus(GameManager.Instance.AuthenticationMessage);
    }

    void OnDestroy()
    {
        if (joinButton != null) joinButton.onClick.RemoveListener(OpenJoin);
        if (loginButton != null) loginButton.onClick.RemoveListener(Login);
        if (resetPasswordButton != null) resetPasswordButton.onClick.RemoveListener(ResetPassword);
    }

    void OpenJoin()
    {
        if (!busy) GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Join);
    }

    async void Login()
    {
        string email = NormalizeEmail(emailInput.text);
        string password = passwordInput.text;
        if (!IsValidEmail(email)) { SetStatus("올바른 이메일을 입력해 주세요."); return; }
        if (string.IsNullOrEmpty(password)) { SetStatus("비밀번호를 입력해 주세요."); return; }
        if (!Begin("로그인 중...")) return;
        try
        {
            AuthResult authResult = await GameManager.Instance.AccountService.SignInAsync(email, password);
            if (authResult.User == null) throw new InvalidOperationException();
            UserProfileLoadResult profile = await GameManager.Instance.RefreshAuthenticatedUserAsync();
            if (profile.Status != UserProfileLoadStatus.Found)
                throw new InvalidOperationException("가입이 완료된 사용자 데이터를 찾을 수 없습니다.");
            GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
        }
        catch (Exception exception) { SetStatus(ToUserMessage(exception, "로그인에 실패했습니다.")); }
        finally { End(); }
    }

    async void ResetPassword()
    {
        string email = NormalizeEmail(emailInput.text);
        if (!IsValidEmail(email)) { SetStatus("비밀번호 재설정에는 가입 이메일을 입력해 주세요."); return; }
        if (!Begin("비밀번호 재설정 메일을 보내는 중...")) return;
        try { await GameManager.Instance.AccountService.SendPasswordResetEmailAsync(email); SetStatus("비밀번호 재설정 메일을 보냈습니다."); }
        catch (Exception exception) { SetStatus(ToUserMessage(exception, "재설정 메일 전송에 실패했습니다.")); }
        finally { End(); }
    }

    bool Begin(string message)
    {
        if (busy) return false;
        busy = true; SetInteractable(false); SetStatus(message); return true;
    }
    void End() { busy = false; SetInteractable(true); }
    void SetInteractable(bool value) { joinButton.interactable = value; loginButton.interactable = value; resetPasswordButton.interactable = value; }
    void SetStatus(string message) { statusText.text = message ?? string.Empty; statusText.gameObject.SetActive(!string.IsNullOrEmpty(message)); }
    bool ValidateReferences()
    {
        bool valid = emailInput != null && passwordInput != null && joinButton != null && loginButton != null && resetPasswordButton != null && statusText != null;
        if (!valid) Debug.LogError("LoginUI Inspector 참조를 모두 설정해 주세요.", this);
        return valid;
    }
    public static string NormalizeEmail(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToLowerInvariant();
    public static bool IsValidEmail(string email) { try { return !string.IsNullOrWhiteSpace(email) && new MailAddress(email).Address == email; } catch { return false; } }
    public static string ToUserMessage(Exception exception, string fallback)
    {
        while (exception is AggregateException && exception.InnerException != null) exception = exception.InnerException;
        if (exception is AccountServerException server) return server.Message;
        if (!(exception is FirebaseException firebase)) return fallback;
        switch ((AuthError)firebase.ErrorCode)
        {
            case AuthError.WrongPassword:
            case AuthError.UserNotFound: return "이메일 또는 비밀번호를 확인해 주세요.";
            case AuthError.NetworkRequestFailed: return "네트워크 연결을 확인해 주세요.";
            case AuthError.TooManyRequests: return "요청이 너무 많습니다. 잠시 후 다시 시도해 주세요.";
            default: return fallback;
        }
    }
}
