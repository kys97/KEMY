using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public List<Quiz> QuizList;

    
    void Start()
    {
        QuizList = new CsvManager().Read_Quiz_Csv();
    }

    void Update()
    {
        
    }
}
