using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinUI : MonoBehaviour
{
    [Header("Scroll forwarding")]
    [SerializeField] ScrollRect joinScrollRect;
    [Header("Nickname")]
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] TMP_Text nicknameErrorText;
    [SerializeField] Button nicknameCheckButton;
    [Header("Phone")]
    [SerializeField] TMP_InputField phoneInput;
    [SerializeField] TMP_Text phoneErrorText;
    [Header("Email verification")]
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_Text emailErrorText;
    [SerializeField] Button sendCodeButton;
    [SerializeField] TMP_InputField verificationCodeInput;
    [SerializeField] TMP_Text verificationErrorText;
    [SerializeField] Button verifyCodeButton;
    [Header("Password")]
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_Text passwordErrorText;
    [SerializeField] TMP_InputField passwordConfirmInput;
    [SerializeField] TMP_Text passwordConfirmErrorText;
    [Header("Submit")]
    [SerializeField] Button joinButton;

    FirebaseAccountService accountService;
    string sessionId, signupGrant, verifiedEmail, checkedNickname, checkedPhone;
    float codeExpiresAt, resendAvailableAt;
    int nicknameRequest, phoneRequest, emailRequest;
    bool busy;

    void Awake()
    {
        bool referencesValid = ValidateReferences();
        bool scrollConfigured = ConfigureScrollForwarding();
        if (!referencesValid || !scrollConfigured) { enabled = false; return; }
        nicknameCheckButton.onClick.AddListener(CheckNickname);
        sendCodeButton.onClick.AddListener(SendCode);
        verifyCodeButton.onClick.AddListener(VerifyCode);
        joinButton.onClick.AddListener(CompleteRegistration);
        nicknameInput.onValueChanged.AddListener(OnNicknameChanged);
        phoneInput.onValueChanged.AddListener(OnPhoneChanged);
        phoneInput.onEndEdit.AddListener(CheckPhone);
        emailInput.onValueChanged.AddListener(OnEmailChanged);
        passwordInput.onValueChanged.AddListener(OnPasswordChanged);
        passwordConfirmInput.onValueChanged.AddListener(OnPasswordConfirmChanged);
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordConfirmInput.contentType = TMP_InputField.ContentType.Password;
        HideAllErrors();
    }

    void Start()
    {
        accountService = GameManager.Instance.AccountService;
        if (accountService.AccountServer == null)
        {
            string baseUrl = "https://asia-northeast3-" + Firebase.FirebaseApp.DefaultInstance.Options.ProjectId + ".cloudfunctions.net";
            accountService.ConfigureAccountServer(baseUrl);
        }
    }

    void OnDestroy()
    {
        if (nicknameCheckButton != null) nicknameCheckButton.onClick.RemoveListener(CheckNickname);
        if (sendCodeButton != null) sendCodeButton.onClick.RemoveListener(SendCode);
        if (verifyCodeButton != null) verifyCodeButton.onClick.RemoveListener(VerifyCode);
        if (joinButton != null) joinButton.onClick.RemoveListener(CompleteRegistration);
        if (nicknameInput != null) nicknameInput.onValueChanged.RemoveListener(OnNicknameChanged);
        if (phoneInput != null) { phoneInput.onValueChanged.RemoveListener(OnPhoneChanged); phoneInput.onEndEdit.RemoveListener(CheckPhone); }
        if (emailInput != null) emailInput.onValueChanged.RemoveListener(OnEmailChanged);
        if (passwordInput != null) passwordInput.onValueChanged.RemoveListener(OnPasswordChanged);
        if (passwordConfirmInput != null) passwordConfirmInput.onValueChanged.RemoveListener(OnPasswordConfirmChanged);
    }

    void Update()
    {
        if (string.IsNullOrEmpty(sessionId) || signupGrant != null) return;
        int remaining = Mathf.Max(0, Mathf.CeilToInt(codeExpiresAt - Time.realtimeSinceStartup));
        int resend = Mathf.Max(0, Mathf.CeilToInt(resendAvailableAt - Time.realtimeSinceStartup));
        SetError(emailErrorText, "인증코드 " + FormatSeconds(remaining) + (resend > 0 ? " · 재전송 " + resend + "초" : " · 재전송 가능"));
        sendCodeButton.interactable = !busy && resend == 0;
    }

    async void CheckNickname()
    {
        string nickname = NormalizeNickname(nicknameInput.text);
        string error = ValidateNickname(nickname);
        if (error != null) { SetError(nicknameErrorText, error); return; }
        int request = ++nicknameRequest;
        SetError(nicknameErrorText, "중복 확인 중...");
        try
        {
            AvailabilityResponse result = await accountService.AccountServer.CheckAvailabilityAsync("nickname", nickname);
            if (request != nicknameRequest || NormalizeNickname(nicknameInput.text) != nickname) return;
            checkedNickname = result.available ? nickname : null;
            SetError(nicknameErrorText, result.available ? "사용 가능한 닉네임입니다." : "이미 사용 중인 닉네임입니다.");
        }
        catch (Exception e) { if (request == nicknameRequest) SetError(nicknameErrorText, LoginUI.ToUserMessage(e, "닉네임 중복 확인에 실패했습니다.")); }
    }

    async void CheckPhone(string _)
    {
        string phone;
        string error = NormalizePhone(phoneInput.text, out phone);
        if (error != null) { SetError(phoneErrorText, error); return; }
        int request = ++phoneRequest;
        SetError(phoneErrorText, "중복 확인 중...");
        try
        {
            AvailabilityResponse result = await accountService.AccountServer.CheckAvailabilityAsync("phone", phone);
            string current; if (NormalizePhone(phoneInput.text, out current) != null || current != phone || request != phoneRequest) return;
            checkedPhone = result.available ? phone : null;
            SetError(phoneErrorText, result.available ? "사용 가능한 번호입니다." : "이미 등록된 휴대폰 번호입니다.");
        }
        catch (Exception e) { if (request == phoneRequest) SetError(phoneErrorText, LoginUI.ToUserMessage(e, "휴대폰 번호 중복 확인에 실패했습니다.")); }
    }

    async void SendCode()
    {
        string email = LoginUI.NormalizeEmail(emailInput.text);
        if (!LoginUI.IsValidEmail(email)) { SetError(emailErrorText, "올바른 이메일을 입력해 주세요."); return; }
        if (!Begin()) return;
        int request = ++emailRequest;
        SetError(emailErrorText, "인증코드를 보내는 중...");
        try
        {
            SendCodeResponse response = await accountService.AccountServer.SendVerificationCodeAsync(email, sessionId);
            if (request != emailRequest || LoginUI.NormalizeEmail(emailInput.text) != email) return;
            sessionId = response.sessionId; signupGrant = null; verifiedEmail = null; verificationCodeInput.text = string.Empty;
            codeExpiresAt = Time.realtimeSinceStartup + response.expiresInSeconds;
            resendAvailableAt = Time.realtimeSinceStartup + response.resendAfterSeconds;
            SetError(emailErrorText, "가입 가능 여부와 관계없이 입력한 메일함을 확인해 주세요.");
        }
        catch (Exception e) { SetError(emailErrorText, LoginUI.ToUserMessage(e, "인증 메일 전송에 실패했습니다.")); }
        finally { End(); }
    }

    async void VerifyCode()
    {
        string email = LoginUI.NormalizeEmail(emailInput.text);
        string code = verificationCodeInput.text;
        if (string.IsNullOrEmpty(sessionId)) { SetError(verificationErrorText, "먼저 인증코드를 전송해 주세요."); return; }
        if (!Regex.IsMatch(code ?? string.Empty, "^[0-9]{6}$")) { SetError(verificationErrorText, "6자리 숫자 인증코드를 입력해 주세요."); return; }
        if (!Begin()) return;
        int request = ++emailRequest;
        string requestedSession = sessionId;
        SetError(verificationErrorText, "인증코드를 확인하는 중...");
        try
        {
            VerifyCodeResponse response = await accountService.AccountServer.VerifyCodeAsync(email, requestedSession, code);
            if (request != emailRequest || sessionId != requestedSession || LoginUI.NormalizeEmail(emailInput.text) != email) return;
            signupGrant = response.signupGrant; verifiedEmail = email;
            SetError(verificationErrorText, "이메일 인증이 완료되었습니다.");
        }
        catch (Exception e) { SetError(verificationErrorText, LoginUI.ToUserMessage(e, "인증코드 확인에 실패했습니다.")); }
        finally { End(); }
    }

    async void CompleteRegistration()
    {
        string nickname = NormalizeNickname(nicknameInput.text);
        string phone; string phoneError = NormalizePhone(phoneInput.text, out phone);
        string email = LoginUI.NormalizeEmail(emailInput.text);
        bool valid = true;
        string nicknameError = ValidateNickname(nickname);
        if (nicknameError != null || checkedNickname != nickname) { SetError(nicknameErrorText, nicknameError ?? "닉네임 중복 확인이 필요합니다."); valid = false; }
        if (phoneError != null || checkedPhone != phone) { SetError(phoneErrorText, phoneError ?? "휴대폰 번호 중복 확인이 필요합니다."); valid = false; }
        if (!LoginUI.IsValidEmail(email) || signupGrant == null || verifiedEmail != email) { SetError(emailErrorText, "현재 이메일의 인증을 완료해 주세요."); valid = false; }
        string passwordError = ValidatePassword(passwordInput.text); SetError(passwordErrorText, passwordError);
        string confirmError = passwordInput.text == passwordConfirmInput.text ? null : "비밀번호가 일치하지 않습니다."; SetError(passwordConfirmErrorText, confirmError);
        if (passwordError != null || confirmError != null) valid = false;
        if (!valid || !Begin()) return;
        try
        {
            CustomTokenResponse response = await accountService.AccountServer.CompleteRegistrationAsync(new CompleteRegistrationRequest
            { email = email, sessionId = sessionId, signupGrant = signupGrant, nickname = nickname, phone = phone, password = passwordInput.text });
            AuthResult auth = await accountService.SignInWithCustomTokenAsync(response.customToken);
            if (auth.User == null || (await GameManager.Instance.RefreshAuthenticatedUserAsync()).Status != UserProfileLoadStatus.Found) throw new InvalidOperationException();
            GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
        }
        catch (Exception e) { SetError(emailErrorText, LoginUI.ToUserMessage(e, "가입을 완료하지 못했습니다.")); }
        finally { End(); }
    }

    void OnNicknameChanged(string _) { checkedNickname = null; nicknameRequest++; SetError(nicknameErrorText, null); }
    void OnPhoneChanged(string _) { checkedPhone = null; phoneRequest++; SetError(phoneErrorText, null); }
    void OnEmailChanged(string value)
    {
        emailRequest++;
        if (sessionId != null && LoginUI.NormalizeEmail(value) != verifiedEmail)
        { sessionId = null; signupGrant = null; verifiedEmail = null; verificationCodeInput.text = string.Empty; SetError(verificationErrorText, null); }
        SetError(emailErrorText, null);
    }
    void OnPasswordChanged(string value) { SetError(passwordErrorText, ValidatePassword(value)); OnPasswordConfirmChanged(passwordConfirmInput.text); }
    void OnPasswordConfirmChanged(string value) { SetError(passwordConfirmErrorText, string.IsNullOrEmpty(value) || value == passwordInput.text ? null : "비밀번호가 일치하지 않습니다."); }
    bool Begin() { if (busy) return false; busy = true; SetInteractable(false); return true; }
    void End() { busy = false; SetInteractable(true); }
    void SetInteractable(bool value)
    {
        nicknameCheckButton.interactable = value; verifyCodeButton.interactable = value; joinButton.interactable = value;
        sendCodeButton.interactable = value && Time.realtimeSinceStartup >= resendAvailableAt;
    }
    static string NormalizeNickname(string value) => (value ?? string.Empty).Trim().Normalize(NormalizationForm.FormKC);
    static string ValidateNickname(string value)
    {
        int count = 0;
        for (int i = 0; i < value.Length; i++)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(value, i);
            if (category == UnicodeCategory.Control || category == UnicodeCategory.Format) return "닉네임에 줄바꿈·제어문자를 사용할 수 없습니다.";
            if (char.IsHighSurrogate(value[i]) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1])) i++;
            count++;
        }
        return count < 1 || count > 8 ? "닉네임은 1~8글자로 입력해 주세요." : null;
    }
    static string NormalizePhone(string raw, out string normalized)
    {
        string value = raw ?? string.Empty; normalized = null;
        if (!Regex.IsMatch(value, "^[0-9 -]+$")) return "숫자, 공백, 하이픈만 입력해 주세요.";
        string candidate = value.Replace("-", string.Empty).Replace(" ", string.Empty);
        if (!Regex.IsMatch(candidate, "^010[0-9]{8}$")) return "010 휴대폰 번호를 확인해 주세요.";
        normalized = candidate; return null;
    }
    static string ValidatePassword(string value)
    {
        if (value == null || value.Length < 8 || value.Length > 20) return "비밀번호는 8~20자여야 합니다.";
        if (!Regex.IsMatch(value, "^[\\x21-\\x7E]+$")) return "비밀번호에는 공백·한글을 사용할 수 없습니다.";
        if (!Regex.IsMatch(value, "[A-Za-z]") || !Regex.IsMatch(value, "[0-9]")) return "영문자와 숫자를 각각 1개 이상 포함해 주세요.";
        return null;
    }
    static void SetError(TMP_Text text, string message) { text.text = message ?? string.Empty; text.gameObject.SetActive(!string.IsNullOrEmpty(message)); }
    void HideAllErrors() { SetError(nicknameErrorText, null); SetError(phoneErrorText, null); SetError(emailErrorText, null); SetError(verificationErrorText, null); SetError(passwordErrorText, null); SetError(passwordConfirmErrorText, null); }
    static string FormatSeconds(int seconds) => (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
    bool ConfigureScrollForwarding()
    {
        if (joinScrollRect == null) joinScrollRect = GetComponent<ScrollRect>();
        if (joinScrollRect == null)
        {
            Debug.LogError("JoinUI와 같은 오브젝트에 ScrollRect를 연결해 주세요.", this);
            return false;
        }
        if (joinScrollRect.content == null)
        {
            Debug.LogError("Join ScrollRect의 Content 참조를 연결해 주세요.", joinScrollRect);
            return false;
        }

        HashSet<GameObject> configured = new HashSet<GameObject>();
        TMP_InputField[] inputFields = joinScrollRect.content.GetComponentsInChildren<TMP_InputField>(true);
        for (int i = 0; i < inputFields.Length; i++)
            ConfigureScrollTarget(inputFields[i].gameObject, configured);

        Button[] buttons = joinScrollRect.content.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
            ConfigureScrollTarget(buttons[i].gameObject, configured);

        return true;
    }
    void ConfigureScrollTarget(GameObject target, HashSet<GameObject> configured)
    {
        if (target == null || !configured.Add(target)) return;
        if (target.GetComponentInParent<ScrollRect>(true) != joinScrollRect) return;

        JoinScrollEventTrigger trigger = target.GetComponent<JoinScrollEventTrigger>();
        if (trigger == null) trigger = target.AddComponent<JoinScrollEventTrigger>();
        trigger.Initialize(joinScrollRect);
    }
    bool ValidateReferences()
    {
        bool valid = nicknameInput && nicknameErrorText && nicknameCheckButton && phoneInput && phoneErrorText && emailInput && emailErrorText && sendCodeButton && verificationCodeInput && verificationErrorText && verifyCodeButton && passwordInput && passwordErrorText && passwordConfirmInput && passwordConfirmErrorText && joinButton;
        if (!valid) Debug.LogError("JoinUI Inspector 참조를 모두 설정해 주세요.", this);
        return valid;
    }
}
