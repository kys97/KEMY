using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using System.Collections.Generic;

public class LoginUI : MonoBehaviour
{
    private string uid = null;

    void Start()
    {
        //Join Btn Set
        transform.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate
        {
            string email = transform.GetChild(3).GetComponent<TMP_InputField>().text;
            string pw = transform.GetChild(5).GetComponent<TMP_InputField>().text;
            UserJoin(email, pw);
        });

        //Login Btn Set
        transform.GetChild(7).GetComponent<Button>().onClick.AddListener(delegate
        {
            UserLogin(transform.GetChild(3).GetComponent<TMP_InputField>().text,
                transform.GetChild(5).GetComponent<TMP_InputField>().text);
        });
    }

    void UserJoin(string email, string pw)
    {
        GameManager.Instance.auth.CreateUserWithEmailAndPasswordAsync(email, pw).ContinueWith(task =>
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
            Debug.Log("회원가입 완료");
        });
    }

    void UserLogin(string email, string pw)
    {
        GameManager.Instance.auth.SignInWithEmailAndPasswordAsync(email, pw).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                return;
            }

            uid = task.Result.User.UserId;
        });
    }

    private void Update()
    {
        if(uid != null)
        {
            GetUserInfo();
        }
    }

    void GetUserInfo()
    {
        string uid = this.uid;
        this.uid = null;

        GameManager.Instance.userDB.Child(uid).GetValueAsync().ContinueWithOnMainThread(
            task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("ReadErr");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("로그인 정보 있음");
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        Debug.Log("User 정보 있음");
                        GameManager.Instance.Data.info.login = true;
                        GameManager.Instance.Data.info.uid = uid;
                        foreach (var data in snapshot.Children)
                        {
                            switch (data.Key.ToString())
                            {
                                case "name": GameManager.Instance.Data.info.name = data.Value.ToString(); break;
                                case "coin": GameManager.Instance.Data.info.coin = int.Parse(data.Value.ToString()); break;
                                case "heart": GameManager.Instance.Data.info.heart = int.Parse(data.Value.ToString()); break;

                                case "skinSet": GameManager.Instance.Data.avatar_info.skin = data.Value.ToString(); break;
                                case "faceSet": GameManager.Instance.Data.avatar_info.face = data.Value.ToString(); break;

                                case "skinList": GameManager.Instance.Data.inven.skin = new List<string>(data.Value.ToString().Split(",")); break;
                                case "faceList": GameManager.Instance.Data.inven.face = new List<string>(data.Value.ToString().Split(",")); break;
                            }
                        }

                        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
                    }
                    else
                    {
                        Debug.Log("User 정보 없음");
                        GameManager.Instance.Data.info.uid = uid;
                        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.SetName);
                    }
                }
            }
        );
    }
}
