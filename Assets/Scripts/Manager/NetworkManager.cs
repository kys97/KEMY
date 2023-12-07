using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> lobby_list = new List<RoomInfo>();
    public List<string> lobby_name_list = new List<string>();

    public static string RoomName;
    public static int MaxCount;

    Transform parent;//Content transform
    GameObject lobby_prefab;//Lobby Btn Prefab

    #region 서버 연결 및 초기화
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();//서버 연결
    }

    public override void OnConnectedToMaster()//서버 연결 후 callback으로 호출
    {
        Debug.Log("서버 연결 완료");

        //변수 셋팅
        lobby_prefab = Resources.Load<GameObject>("Prefabs/KampRoomBtn");
        parent = null;

        //NickName 설정
        //PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.Data.info.name;
    }
    #endregion

    public override void OnLeftLobby()
    {
        parent = null;
    }

    public void ChatOn()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if(parent == null)
            parent = GameObject.FindGameObjectWithTag("Lev1").transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0);


        LobbyListUpdate();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for(int i=0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!lobby_list.Contains(roomList[i]))
                    lobby_list.Add(roomList[i]);
                else
                    lobby_list[lobby_list.IndexOf(roomList[i])] = roomList[i];
            }
            else if (lobby_list.IndexOf(roomList[i]) != -1)
                lobby_list.RemoveAt(lobby_list.IndexOf(roomList[i]));
        }

        LobbyListUpdate();
    }

    private void LobbyListUpdate()
    {
        //lobby_btn_list.Clear();

        for (int i=0; i < lobby_list.Count; i++)
        {
            GameObject kamp = Instantiate(lobby_prefab, parent);
            kamp.GetComponent<KampBtn>().Set(lobby_list[i].Name, lobby_list[i].PlayerCount, lobby_list[i].MaxPlayers);
            lobby_name_list.Add(lobby_list[i].Name);
            //lobby_btn_list.Add (kamp);
        }
    }



    public void CreateKamp(string name, int cnt)
    {
        //Kamp 생성
        RoomName = name;
        MaxCount = cnt;
        PhotonNetwork.CreateRoom(name, new RoomOptions { MaxPlayers = cnt });
    }
    public override void OnJoinedRoom()
    {
        RoomName = PhotonNetwork.CurrentRoom.Name;
        GameManager.Instance.UImanager.Goto_KampScene();

        //Kamp 초기화 함수

        //Kamp Name
        //PhotonNetwork.CurrentRoom.Name

        //Kamp 접속 인원
        //PhotonNetwork.CurrentRoom.PlayerCount

        //Kamp 최대 인원
        //PhotonNetwork.CurrentRoom.MaxPlayers
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        GameObject.FindGameObjectWithTag("Lev2").transform.GetChild(0).GetComponent<CreateKampUI>().FailedCreateKamp();
    }


    public void JoinKamp(string name)
    {
        RoomName = name;
        GameManager.Instance.UImanager.Goto_KampScene();
    }

    

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //누군가 방에 들어왔을 때 호출
        //접속인원 수정
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }
}
