using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Resources;
using TMPro;
using UnityEngine;

public class KampManager : MonoBehaviourPunCallbacks
{
    public static int PlayerCount = 1;

    TMP_Text count, name;

    private void Start()
    {
        PhotonNetwork.JoinOrCreateRoom(NetworkManager.RoomName, new RoomOptions { MaxPlayers = NetworkManager.MaxCount }, null);

        //UI Set
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Kamp);
        name = GameObject.FindWithTag("Lev1").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        count = GameObject.FindWithTag("Lev1").transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    public override void OnJoinedRoom()
    {
        //Avatar Set
        GameObject MyAvatar = PhotonNetwork.Instantiate("Prefabs/KampCat", Vector3.zero, Quaternion.identity);
        //Skin Set
        Material[] skin = MyAvatar.transform.GetChild(2).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials;
        skin[0] = GameManager.Instance.Resourcesmanager.ItemMaterials[GameManager.Instance.Data.avatar_info.skin];
        skin[1] = GameManager.Instance.Resourcesmanager.ItemMaterials[GameManager.Instance.Data.avatar_info.face];
        MyAvatar.transform.GetChild(2).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials = skin;
        SetRoomText();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomText();
    }

    [PunRPC]
    public void SetRoomText()
    {
        name.text = PhotonNetwork.CurrentRoom.Name;
        count.text = "(<color=#4A90E2>" + PhotonNetwork.CurrentRoom.PlayerCount + "</color>/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
    }
}
