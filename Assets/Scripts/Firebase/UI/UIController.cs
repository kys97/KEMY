using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public FirebaseController firebase;

    public Button signInBtn;
    public Button signOutBtn;
    public TextMeshProUGUI userIDLabel;

    public TMP_InputField usernameInput;
    public TMP_InputField messageInput;

    void Start()
    {
        // 처음 켰을 때 아직 로그인이 안된 상태를 가정
        UpdateUserInfo(false);
    }

    public void UpdateUserInfo(bool isSigned, string userID = "")
    {
        if (isSigned)
        {
            signInBtn.interactable = false;
            signOutBtn.interactable = true;
            userIDLabel.text = "UID : " + userID;
        }
        // 앱을 처음 켰거나 탈퇴를 눌렀을 때
        else 
        {
            signInBtn.interactable = true;
            signOutBtn.interactable = false;
            userIDLabel.text = "SignOut... ";
        }
    }

    public void SendChat()
    {
        string username = usernameInput.text;
        string message = messageInput.text;
        firebase.SendChatMessage(username, message);
    }
}
