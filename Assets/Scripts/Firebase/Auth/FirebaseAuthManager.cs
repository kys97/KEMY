using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    public TMP_InputField email;
    public TMP_InputField password;

    private bool isLonInComplete = false;
    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Create()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("ȸ������ ���");
                return;
            }
            if (task.IsFaulted)
            {
                //�������� > �̸����� ������, ��й�ȣ �ʹ� ����, �̹� ���Ե� �̸���
                Debug.LogError("ȸ������ ����");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            Debug.LogError("ȸ������ �Ϸ�");
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�α��� ���");
                return;
            }
            if (task.IsFaulted)
            {
                //�������� > �̸����� ������, ��й�ȣ �ʹ� ����, �̹� ���Ե� �̸���
                Debug.LogError("�α��� ����");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            Debug.Log("�α��� �Ϸ�");
            isLonInComplete = true;
        });
        Change();
    }
    public void Change()
    {
        if(isLonInComplete)
        {
            SceneManager.LoadScene("Home");
        }
    }
    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("�α׾ƿ�");
    }
}
