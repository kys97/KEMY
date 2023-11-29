using Autodesk.Fbx;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Setting")]
    public int QuizTotalCount;
    public int SolveCount;
    public float SolveTime;
    public int Level;

    public List<Quiz> QuizList;

    [Header("Quiz Data")]
    public int Score;
    public List<QuizResult> ResultPull;
    [Serializable]
    public struct QuizResult
    {
        public bool Result;
        public string Quiz;
        public string Select;
        public string Answer;
    }

    private static QuizManager instance = null;
    public static QuizManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        QuizList = new CsvManager().Read_Quiz_Csv();
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.QuizReady);

        Init();
    }

    void Update()
    {
        
    }

    private void Init()
    {
        SolveCount = 0;
        Score = 0;

        //화면 셋팅
    }
}
