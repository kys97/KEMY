using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    void Start()
    {
        //Bottom Btn Image Setting
        GameObject.FindGameObjectWithTag("Bottom").transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.home_select.ToString()];
        GameObject.FindGameObjectWithTag("Bottom").transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.chat.ToString()];
        GameObject.FindGameObjectWithTag("Bottom").transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.book.ToString()];

        //Avatar Setting
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);

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
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Bottom").SetActive(false);
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Shop);
    }
}
