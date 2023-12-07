using Photon.Pun;
using Photon.Realtime;
using System.Resources;
using UnityEngine;

public class KampManager : MonoBehaviourPunCallbacks
{
    
    void Awake()
    {
        
    }

    private void Start()
    {
        //Avatar Set
        GameObject MyAvatar = PhotonNetwork.Instantiate("Prefabs/KampCat", Vector3.zero, Quaternion.identity);

        Material[] skin = MyAvatar.transform.GetChild(2).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials;
        skin[0] = GameManager.Instance.Resourcesmanager.ItemMaterials[GameManager.Instance.Data.avatar_info.skin];
        skin[1] = GameManager.Instance.Resourcesmanager.ItemMaterials[GameManager.Instance.Data.avatar_info.face];
        MyAvatar.transform.GetChild(2).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials = skin;

        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;
        MyAvatar.transform.parent = parent;
        MyAvatar.transform.position = parent.position;
        MyAvatar.transform.localScale = new Vector3(1, 1, 1);
        

        //UI Set
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Kamp);
    }
}
