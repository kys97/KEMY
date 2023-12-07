using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Resources;
using UnityEngine;

public class KampManager : MonoBehaviourPunCallbacks
{
    public static int PlayerCount = 1;

    private void Awake()
    {
        PhotonNetwork.JoinOrCreateRoom(NetworkManager.RoomName, new RoomOptions { MaxPlayers = NetworkManager.MaxCount}, null);
    }

    private void Start()
    {
        //UI Set
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Kamp);
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
        //Transform Set
        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;
        MyAvatar.transform.parent = parent;
        MyAvatar.transform.position = parent.position;
        MyAvatar.transform.localScale = new Vector3(1, 1, 1);
        //Name Set
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }
}
