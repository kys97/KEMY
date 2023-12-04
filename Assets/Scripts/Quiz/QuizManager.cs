using Autodesk.Fbx;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Setting")]
    public int MaxLevel;//�ִ� ���̵� ����
    public int QuizTotalCount;//�� ���� ���� ��
    public int SolveCount;//Ǭ ���� ��
    public float SolveTime;//�� ���� Ǫ�µ� �ɸ��� �ð�
    public int Level;//���� ���̵� ����

    [Header("Quiz")]
    public List<Quiz> QuizList;//��ü ���� ����Ʈ
    public float[] DifficultLevelRate;//heart�߰� �����ϴ� ���̵� ����
    public int[,] LevelIndex;//���� ���̵��� index����


    [Header("Quiz Data")]
    public int Score;//���� ����
    public int Heart;//���� ��Ʈ ��
    public List<QuizResult> ResultPull;//���� ��� ����Ʈ
    [Serializable]
    public struct QuizResult
    {
        public bool Result;
        public Quiz GetQuiz;
    }


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
        QuizList.Sort(compareQuizRate);

        LevelIndex = new int[MaxLevel, 2];
        DifficultLevelRate = new float[MaxLevel];

        int temp = 0;
        for (int i=0; i < MaxLevel; i++)
        {
            int j = QuizList.Count / 3 * (i + 1);

            LevelIndex[i,0] = GetStartIndex(temp);
            LevelIndex[i,1] = GetEndIndex(j);
            temp = j;

            DifficultLevelRate[i] = QuizList[(j / 10) + LevelIndex[i, 0]].Rate;
        }

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

        return prev;
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
        //���� ����
        SolveCount = 0;
        Score = 0;
        Heart = 0;
    }
}
