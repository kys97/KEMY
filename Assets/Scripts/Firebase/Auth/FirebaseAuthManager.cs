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
                Debug.LogError("회원가입 취소");
                return;
            }
            if (task.IsFaulted)
            {
                //실패이유 > 이메일이 비정상, 비밀번호 너무 간단, 이미 가입된 이메일
                Debug.LogError("회원가입 실패");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            Debug.LogError("회원가입 완료");
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                //실패이유 > 이메일이 비정상, 비밀번호 너무 간단, 이미 가입된 이메일
                Debug.LogError("로그인 실패");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            Debug.Log("로그인 완료");
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
        Debug.Log("로그아웃");
    }
}
