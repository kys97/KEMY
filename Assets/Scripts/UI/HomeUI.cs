using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    void Start()
    {
        //Kamp Btn Setting
        //Read Kamp Join Data
        /*
        if((Kamp Join Data))
        {
            transform.GetChild(0).gameObject.active = false;
            //transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_KampScene);
        }
        else
        {
            //transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_KampScene);
            transform.GetChild(1).gameObject.active = false;
        }
        */

        //User Name(=NickName) Setting
        //Read User NickName
        //transform.GetChild(2).GetComponent<TMP_Text>().text = (User NickName);

        //Inven Btn Setting
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(OpenInvenBtn);

        //Shop Btn Setting
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(OpenShopBtn);
    }

    void OpenInvenBtn()
    {
        //올라오는 효과
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Inventory);
    }

    void OpenShopBtn()
    {
        //올라오는 효과
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Shop);
    }
}
