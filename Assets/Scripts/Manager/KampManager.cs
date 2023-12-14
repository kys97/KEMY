using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KampManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView PV;

    TMP_Text KampName_txt;
    TMP_Text UserCount_txt;
    Transform MessageParent;
    ScrollRect Scroll;
    Queue<GameObject> MessageQueue = new Queue<GameObject>();

    #region Singleton
    private static KampManager instance = null;
    public static KampManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region UI 변수 동적 할당
    public void SetMessageParent(Transform parent) { MessageParent = parent; }
    public void SetScroll(ScrollRect scroll) {  Scroll = scroll; }
    public void SetKampText(TMP_Text name, TMP_Text count) {  KampName_txt = name; UserCount_txt = count; }
    #endregion

    private void Start()
    {
        PhotonNetwork.JoinOrCreateRoom(NetworkManager.RoomName, new RoomOptions { MaxPlayers = NetworkManager.MaxCount }, null);


        //UI Set
        if (GameObject.FindWithTag("Lev1").transform.childCount == 0)
        {
            GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Kamp);
        }
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

        //Kamp Set
        KampName_txt.text = PhotonNetwork.CurrentRoom.Name;
        SetPlayerCount();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetPlayerCount();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetPlayerCount();
    }

    private void SetPlayerCount()
    {
        //Player Count Set
        UserCount_txt.text = "(<color=#4A90E2>" + PhotonNetwork.CurrentRoom.PlayerCount + "</color>/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
    }

    public void SendBtnClick(string text)
    {
        PV.RPC("SendMessageRPC", RpcTarget.All, GameManager.Instance.Data.info.name, text);
    }

    [PunRPC]
    public void SendMessageRPC(string user, string text)
    {
        GameObject new_message;

        if (MessageQueue.Count < 30)
        {
            new_message = Instantiate(Resources.Load<GameObject>("Prefabs/MessageBox"));
        }
        else
        {
            new_message = MessageQueue.Dequeue();
            new_message.transform.parent = transform;
        }

        new_message.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        new_message.transform.GetChild(1).GetComponent<TMP_Text>().text = user;
        new_message.transform.parent = MessageParent;
        new_message.GetComponent<RectTransform>().localScale = Vector3.one;
        MessageQueue.Enqueue(new_message);

        Scroll.verticalNormalizedPosition = -0.5f;
    }
}
