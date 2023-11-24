using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    TMP_Text coin_txt, heart_txt;

    void Start()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);

        //Coint Text Setting
        coin_txt = transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        coin_txt.text = GameManager.Instance.Data.info.coin.ToString();
        heart_txt = transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();
        heart_txt.text = GameManager.Instance.Data.info.heart.ToString();

        //Bottom Setting
        transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home));
        //transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Chat));
        transform.GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Game));
    }

    void Update()
    {
        
    }

    public void UpdateCoin()
    {
        coin_txt.text = GameManager.Instance.Data.info.coin.ToString();
        heart_txt.text = GameManager.Instance.Data.info.heart.ToString();
    }
}
