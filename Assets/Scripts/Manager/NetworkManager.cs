using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();//���� ����
    }

    public override void OnConnectedToMaster()//���� ���� �� callback���� ȣ��
    {
        //�游���
        PhotonNetwork.CreateRoom("KampName", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;
        GameObject MyAvatar = PhotonNetwork.Instantiate("Prefabs/KampCat", parent.position, Quaternion.identity);
        MyAvatar.transform.localScale = parent.localScale;
        MyAvatar.transform.parent = parent;
    }
}
