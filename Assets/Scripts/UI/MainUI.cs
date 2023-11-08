using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);

        //Coint Text Setting
        //Read Coin Data
        //transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = (Coin Data);

        //Heart Text Setting
        //Read Heart Data
        //transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = (Heart Data);

        //Bottom Setting
        transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(HomeBtnClick);
        //transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Chat));
        transform.GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(GameBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HomeBtnClick()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);

        //Btn
        transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites["home_select"];
        transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites["chat"];
        transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites["book"];
    }

    void GameBtnClick()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Game);

        //Btn
        transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites["home"];
        transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites["chat"];
        transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites["book_select"];
    }
}
