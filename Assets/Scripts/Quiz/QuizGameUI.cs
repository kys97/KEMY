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

    private float currentTime;//현재 시간 : QuizManager.SolveTime 부터 0까지 줄어듬

    private Quiz Answer;//현재 풀고 있는 문제 정답
    private int answer_option_num;//선택지 영어들 index : TODO : 추후 내가 잘못 고른 선택지 보여줄 때 활용

    void Start()
    {
        SolveCnt_txt = transform.GetChild(0).GetComponent<TMP_Text>();
        Timer_sld = transform.GetChild(1).GetComponent<Slider>();

        Heart_obj = transform.GetChild(2).GetChild(1).gameObject;
        Quiz_txt = transform.GetChild(2).GetChild(2).GetComponent<TMP_Text>();
        Pronunce_txt = transform.GetChild(2).GetChild(3).GetComponent<TMP_Text>();

        Options_txt = new TMP_Text[4];
        for (int i = 0; i < Options_txt.Length; i++)
        {
            int temp_index = i;
            Options_txt[i] = transform.GetChild(3).GetChild(i).GetChild(0).GetComponent<TMP_Text>();
            transform.GetChild(3).GetChild(i).GetComponent<Button>().onClick.AddListener(()=>OptionsBtn(temp_index));
        }


        SolveCnt_txt.text = QuizManager.Instance.SolveCount.ToString() + "/" + QuizManager.Instance.QuizTotalCount.ToString();
        Timer_sld.maxValue = QuizManager.Instance.SolveTime;
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
        currentTime = Timer_sld.maxValue;
        Timer_sld.value = Timer_sld.maxValue;

        if (QuizManager.Instance.SolveCount == QuizManager.Instance.QuizTotalCount)
        {
            //Quiz Finish
            GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.QuizResult);
        }
        else
        {
            //Get New Options
            List<int> option_list = GetRandomQuiz();

            //Set Option Text
            for(int i=0; i < Options_txt.Length; i++)
            {
                Options_txt[i].text = QuizManager.Instance.QuizList[option_list[i]].English;
            }

            //Set Quiz
            answer_option_num = Random.Range(0, Options_txt.Length);
            Answer = QuizManager.Instance.QuizList[option_list[answer_option_num]];
            Quiz_txt.text = Answer.Korean;
            Pronunce_txt.text = Answer.Pronunce;

            //Heart Set
            if(Answer.Rate < QuizManager.Instance.DifficultLevelRate[QuizManager.Instance.Level - 1])
            {
                Heart_obj.SetActive(true);
            }
        }
    }

    List<int> GetRandomQuiz()
    {
        List<int> list = new List<int>();

        int rand = Random.Range(QuizManager.Instance.LevelIndex[QuizManager.Instance.Level - 1, 0], QuizManager.Instance.LevelIndex[QuizManager.Instance.Level - 1, 1]);

        for (int i=0; i < 4; i++)
        {
            if (list.Contains(rand))
            {
                i--;
                rand = Random.Range(QuizManager.Instance.LevelIndex[QuizManager.Instance.Level - 1, 0], QuizManager.Instance.LevelIndex[QuizManager.Instance.Level - 1, 1]);
            }
            else
            {
                list.Add(rand);
            }
        }

        return list;
    }


    void OptionsBtn(int num)
    {
        QuizManager.QuizResult temp_quiz = new QuizManager.QuizResult();
        temp_quiz.GetQuiz = Answer;

        //선택지 결과 적용
        if (num == answer_option_num)
        {
            //Heart
            if (Heart_obj.activeSelf)
            {
                QuizManager.Instance.Heart += 10;
                Heart_obj.SetActive(false);
            }

            //Score
            QuizManager.Instance.Score += 100 + (int)(currentTime * 100);

            //Result Pull
            temp_quiz.Result = true;
            QuizManager.Instance.ResultPull.Add(temp_quiz);
        }
        else
        {
            temp_quiz.Result = false;
            QuizManager.Instance.ResultPull.Add(temp_quiz);
        }

        QuizManager.Instance.SolveCount++;
        NextQuiz();
    }
}
