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
        // ó�� ���� �� ���� �α����� �ȵ� ���¸� ����
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
        // ���� ó�� �װų� Ż�� ������ ��
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
