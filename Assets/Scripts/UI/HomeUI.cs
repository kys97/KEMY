using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    void Start()
    {
        //UI Size Set
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
        
        //Avatar Setting
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);

        //User Name(=NickName) Setting
        transform.GetChild(0).GetComponent<TMP_Text>().text = GameManager.Instance.Data.info.name;

        //Inven Btn Setting
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=> GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Inventory));

        //Shop Btn Setting
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OpenShopBtn);
    }


    void OpenShopBtn()
    {
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Shop);
    }
}
