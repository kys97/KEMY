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
        transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home));
        //transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Chat));
        //transform.GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.StudyGame));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
