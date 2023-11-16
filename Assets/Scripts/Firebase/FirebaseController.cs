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
            {//���� ������ ���� ���� ��� ȣ��
                Debug.LogError("CheckAndFixDependenciesAsync Fail");
            }
        });
    }
    //������������ ���� ������ ���� ��� AuthStateChangedȣ���ؼ� ���������� � ������ �Ǵ��� Ȯ�ΰ���
    private void FirebaseInit()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
    }
    // firebase ���� ���� ��
    private void AuthStateChanged(object sender, EventArgs e)
    {
        FirebaseAuth senderAuth = sender as FirebaseAuth;
        if (senderAuth != null)
        {
            user = senderAuth.CurrentUser;
            // ������ �α����̳� �α׾ƿ����� ���� ���������� ���� ��츦 Ȯ��
            if (user != null)
            {
                //������ ������ ����id Ȯ��
                Debug.Log(user.UserId);
                uiController.UpdateUserInfo(true, user.UserId);
            }
        }
        // �͸����� �α��� ���
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

    //data�� �����ؼ� ���� ������. �޼��� �о����
    public void ReadChatMessage()
    {
        //database�� ������ġ
        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");
        //db�� �о���� ���ؼ� GetValueAsync()�̿�, �ٷ� ������°� �ƴ϶� ��û �� �����
        chatDB.GetValueAsync().ContinueWithOnMainThread(
            task => // task���� ���� �������� ��
            {
                if(task.IsFaulted)
                {
                    Debug.LogError("ReadErr");
                }
                else if(task.IsCompleted)
                {
                    // snapshot�� ������ � data�� �ִ��� Ȯ��
                    DataSnapshot snapshot = task.Result;
                    //��� �޼����� �׿��ִ��� �α׿� �����
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
    //�޼��� ������
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
