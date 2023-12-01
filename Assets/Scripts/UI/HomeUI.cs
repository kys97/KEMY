using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    void Start()
    {
        //Avatar Setting
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);

        //User Name(=NickName) Setting
        //Read User NickName
        //transform.GetChild(2).GetComponent<TMP_Text>().text = (User NickName);

        //Inven Btn Setting
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(()=> GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Inventory));

        //Shop Btn Setting
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(OpenShopBtn);
    }


    void OpenShopBtn()
    {
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Shop);
    }
}
