using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizReadyUI : MonoBehaviour
{
    TMP_Text Level_txt;
    string Level1_color = "#717171", Level2_color = "#6483A6", Level3_color = "#4A90E2";

    void Start()
    {
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);

        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_Home);

        Level_txt = transform.GetChild(1).GetComponent<TMP_Text>();

        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(LevelDownBtn);
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(LevelUpBtn);
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(QuizStartBtn);

    }

    void LevelDownBtn()
    {
        if(QuizManager.Instance.Level > 1)
        {
            QuizManager.Instance.Level --;
            Level_txt.text = "Level " + QuizManager.Instance.Level.ToString();
        }
    }

    void LevelUpBtn()
    {
        if (QuizManager.Instance.Level < QuizManager.Instance.MaxLevel)
        {
            QuizManager.Instance.Level++;
            Level_txt.text = "Level " + QuizManager.Instance.Level.ToString();
        }
    }

    void SetTextColor(string color)
    {
        //TODO
        //Level_txt.color = 
    }

    void QuizStartBtn()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.QuizStart);
    }
}
