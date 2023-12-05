using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour
{
    public TextMeshProUGUI usernameLabel;
    public TextMeshProUGUI messageLabel;

    public void SetMessage(string username, string message)
    {
        usernameLabel.text = username;
        messageLabel.text = message;
    }
}
