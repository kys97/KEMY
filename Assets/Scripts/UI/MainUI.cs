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
        transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = GameManager.Instance.Data.info.coin.ToString();

        //Heart Text Setting
        transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = GameManager.Instance.Data.info.heart.ToString();

        //Bottom Setting
        transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home));
        //transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Chat));
        transform.GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Game));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
