using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;
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

    //data에 접근해서 값을 가져옴. 메세지 읽어오기
    public void ReadChatMessage()
    {
        //database의 시작위치
        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");
        //db값 읽어오기 위해서 GetValueAsync()이용, 바로 갖고오는게 아니라 요청 후 갖고옴
        chatDB.GetValueAsync().ContinueWithOnMainThread(
            task => // task에서 값을 가져갔을 시
            {
                if(task.IsFaulted)
                {
                    Debug.LogError("ReadErr");
                }
                else if(task.IsCompleted)
                {
                    // snapshot의 하위에 어떤 data가 있는지 확인
                    DataSnapshot snapshot = task.Result;
                    //몇개의 메세지가 쌓여있는지 로그에 찍어줌
                    Debug.Log("ChildrenCount : " + snapshot.ChildrenCount);
                    foreach(var message in snapshot.Children)
                    {
                        Debug.Log(message.Key + " " + message.Child("username").Value.ToString() + 
                            " " + message.Child("message").Value.ToString() );
                    }
                }
            }
            );
    }
    //메세지 보내기
    public void SendChatMessage(string username, string message)
    {
        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");
        string key = chatDB.Push().Key;

        Dictionary<string, string> msgDic = new Dictionary<string, string>();
        msgDic.Add("username", username);
        msgDic.Add("message", message);

        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        updateMsg.Add(key, msgDic);

        chatDB.UpdateChildrenAsync(updateMsg).ContinueWithOnMainThread( task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Update Complete");
            }
        }
        );
    }
}
