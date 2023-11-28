using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizGameUI : MonoBehaviour
{
    Slider Timer_sld;
    GameObject Heart_obj;
    TMP_Text SolveCnt_txt, Quiz_txt, Pronunce_txt;
    TMP_Text[] Options_txt;

    private float currentTime;

    void Start()
    {
        SolveCnt_txt = transform.GetChild(1).GetComponent<TMP_Text>();
        Timer_sld = transform.GetChild(2).GetComponent<Slider>();

        Heart_obj = transform.GetChild(3).GetChild(1).gameObject;
        Quiz_txt = transform.GetChild(3).GetChild(2).GetComponent<TMP_Text>();
        Pronunce_txt = transform.GetChild(3).GetChild(3).GetComponent<TMP_Text>();

        Options_txt = new TMP_Text[4];
        for (int i = 0; i < Options_txt.Length; i++)
        {
            Options_txt[i] = transform.GetChild(4).GetChild(i).GetChild(0).GetComponent<TMP_Text>();
        }

        SolveCnt_txt.text = QuizManager.Instance.SolveCount.ToString() + "/" + QuizManager.Instance.QuizTotalCount.ToString();
        Timer_sld.value = Timer_sld.maxValue;
        Heart_obj.SetActive(false);

        NextQuiz();
    }

    void Update()
    {
        //타이머 셋팅
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            Timer_sld.value = currentTime;
        }
        else
        {
            NextQuiz();
        }
    }

    void NextQuiz()
    {
        if (QuizManager.Instance.SolveCount != 0)
            QuizManager.Instance.SolveCount++;

        if (QuizManager.Instance.SolveCount == QuizManager.Instance.QuizTotalCount)
        {
            //Quiz Ending
        }
        else
        {
            currentTime = QuizManager.Instance.SolveTime;

            //New Quiz

        }

        Timer_sld.value = currentTime;
    }
    void AnswerBtn()
    {
        //선택지 결과 적용


        NextQuiz();
    }
}
