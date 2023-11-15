using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine;
using System;
using JetBrains.Annotations;

public class FirebaseController : MonoBehaviour
{
    public UIController uiController;
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if(task.Result == Firebase.DependencyStatus.Available)
            {
                FirebaseInit();
            }
            else
            {//버전 정보가 맞지 않을 경우 호출
                Debug.LogError("CheckAndFixDependenciesAsync Fail");
            }
        });
    }
    //인증정보에서 뭔가 변경이 됐을 경우 AuthStateChanged호출해서 저장함으로 어떤 변경이 되는지 확인가능
    private void FirebaseInit()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
    }
    // firebase 상태 변경 시
    private void AuthStateChanged(object sender, EventArgs e)
    {
        FirebaseAuth senderAuth = sender as FirebaseAuth;
        if (senderAuth != null)
        {
            user = senderAuth.CurrentUser;
            // 유저가 로그인이나 로그아웃으로 현재 유저정보가 없을 경우를 확인
            if (user != null)
            {
                //유저가 있으면 유저id 확인
                Debug.Log(user.UserId);
                uiController.UpdateUserInfo(true, user.UserId);
            }
        }
        // 익명으로 로그인 기능
    }
    public void SignIn()
    {
        SigninAnonymous();
    }
    private Task SigninAnonymous()
    {
        return auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(
            task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Signin Fail");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Signin Complete");
                }
            }
            );
    }
    public void SignOut()
    {
        auth.SignOut();
        uiController.UpdateUserInfo(false);
    }
}
