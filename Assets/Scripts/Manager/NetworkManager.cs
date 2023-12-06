using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();//서버 연결
    }

    public override void OnConnectedToMaster()//서버 연결 후 callback으로 호출
    {
        //방만들기
        PhotonNetwork.JoinOrCreateRoom("KampName", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Prefabs/KampCat", Vector3.zero, Quaternion.identity);
    }
}
