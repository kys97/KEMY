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
        PhotonNetwork.ConnectUsingSettings();//���� ����
    }

    public override void OnConnectedToMaster()//���� ���� �� callback���� ȣ��
    {
        //�游���
        PhotonNetwork.JoinOrCreateRoom("KampName", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Prefabs/KampCat", Vector3.zero, Quaternion.identity);
    }
}
