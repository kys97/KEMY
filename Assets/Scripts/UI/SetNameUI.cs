using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetNameUI : MonoBehaviour
{
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] Button confirmButton;
    [SerializeField] TMP_Text statusText;
    bool busy;

    void Start()
    {
        confirmButton.onClick.AddListener(SetName);
    }

    void OnDestroy()
    {
        if (confirmButton != null) confirmButton.onClick.RemoveListener(SetName);
    }

    async void SetName()
    {
        string nickname = nicknameInput.text.Trim();
        if (busy) return;
        if (nickname.Length < 2 || nickname.Length > 16) { statusText.text = "닉네임은 2~16자로 입력해 주세요."; return; }
        busy = true; confirmButton.interactable = false; statusText.text = "사용자 데이터를 저장하는 중...";
        try
        {
            if (!await GameManager.Instance.CompleteRegistrationAsync(nickname)) throw new InvalidOperationException();
            GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
        }
        catch { statusText.text = "저장하지 못했습니다. 네트워크를 확인한 뒤 다시 눌러 주세요."; }
        finally { busy = false; confirmButton.interactable = true; }
    }

}
