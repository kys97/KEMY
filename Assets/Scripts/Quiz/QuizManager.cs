using Autodesk.Fbx;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;
using static System.Net.Mime.MediaTypeNames;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Setting")]
    public int MaxLevel;//최대 난이도 레벨
    public List<Quiz> QuizList;//전체 퀴즈 리스트

    [Header("Level Setting")]
    public int QuizTotalCount;//총 퀴즈 문제 수
    public float SolveTime;//한 문제 푸는데 걸리는 시간
    [SerializeField] private int level;//현재 난이도 레벨
    public int Level
    {
        get { return level; }
        set
        {
            level = value;

            switch (level)
            {
                case 1:
                    SolveTime = 9f;
                    QuizTotalCount = 10;
                    break;
                case 2:
                    SolveTime = 6f;
                    QuizTotalCount = 20;
                    break;
                case 3:
                    SolveTime = 3f;
                    QuizTotalCount = 30;
                    break;
            }

            repeated_index.Clear();
            for (int i= LevelIndex[value - 1, 0]; i < LevelIndex[value - 1, 1]; i++)
            {
                repeated_index.Add(i);
            }
        }
    }

    [Header("Quiz")]
    public float[] DifficultLevelRate;//heart추가 지급하는 난이도 기준
    public int[,] LevelIndex;//퀴즈 난이도별 index범위


    [Header("Quiz Data")]
    public int SolveCount;//푼 문제 수
    public int Score;//현재 점수  : /10 = 얻은 코인 수
    public int Heart;//얻은 하트 수
    public Dictionary<Quiz, bool> ResultPull;//퀴즈 결과 리스트
    public List<int> repeated_index;//안푼 문제 index


    #region Singleton
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
    #endregion

    void Start()
    {
        QuizList = new CsvManager().Read_Quiz_Csv();
        
        LevelIndex = new int[MaxLevel, 2];
        DifficultLevelRate = new float[MaxLevel];

        ResultPull = new Dictionary<Quiz, bool>();
        repeated_index = new List<int>();

        Init();
    }

    int compareQuizRate(Quiz a, Quiz b)
    {
        return a.Rate > b.Rate ? -1 : 1;
    }

    int GetStartIndex(int i)
    {
        if (i == 0) return 0;

        int prev = i - 1;
        while (QuizList[i].Rate == QuizList[prev].Rate)
        {
            prev--;
            if (prev == 0)
                return prev;
        }

        return prev + 1;
    }

    int GetEndIndex(int i)
    {
        if (i == MaxLevel) return QuizList.Count;

        int next = i;
        while (QuizList[i - 1].Rate == QuizList[next].Rate)
        {
            next++;
            if (next == QuizList.Count)
                return next;
        }

        return next;
    }

    private void Init()
    {
        QuizList.Sort(compareQuizRate);

        //변수 셋팅
        SolveCount = 0;
        Score = 0;
        Heart = 0;
        ResultPull.Clear();
        repeated_index.Clear();

        //Level Set
        int temp = 0;
        for (int i = 0; i < MaxLevel; i++)
        {
            int j = QuizList.Count / 3 * (i + 1);

            LevelIndex[i, 0] = GetStartIndex(temp);
            LevelIndex[i, 1] = GetEndIndex(j);

            temp = j;

            DifficultLevelRate[i] = QuizList[(j / 10) + LevelIndex[i, 0]].Rate;
        }
        Level = 1;
    }
}
