using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvManager
{
    private string ItemFile = "ItemWeight";
    private string QuizFile = "QuizList";


    public List<ItemWeight> Read_ItemWeight_Csv()
    {
        List<ItemWeight> weight_dic = new List<ItemWeight>();
        TextAsset csv = Resources.Load<TextAsset>(ItemFile);
        StringReader reader = new StringReader(csv.text);

        reader.ReadLine();
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();

            var data = line.Split(',');
            var temp = new ItemWeight { name = data[0], type = (Define.item_type)Enum.Parse(typeof(Define.item_type), data[1]), weight = int.Parse(data[2]) };
            GameManager.Instance.TotWeight += int.Parse(data[2]);
            weight_dic.Add(temp);
        }

        return weight_dic;
    }

    public List<Quiz> Read_Quiz_Csv()
    {
        List<Quiz> weight_dic = new List<Quiz>();
        TextAsset csv = Resources.Load<TextAsset>(QuizFile);
        StringReader reader = new StringReader(csv.text);

        reader.ReadLine();
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();

            var data = line.Split(',');
            var temp = new Quiz { Korean = data[0], Pronunce = data[1], English = data[2], Rate = float.Parse(data[3])/*, SolveCnt = int.Parse(data[4])*/ };
            weight_dic.Add(temp);
        }

        return weight_dic;
    }
}

//TODO : 서버 연결 후 퀴즈 결과에 따른 오답률 수정
[Serializable]
public struct Quiz
{
    public string Korean;
    public string Pronunce;
    public string English;
    public float Rate;
    //public int SolveCnt;
}

public struct ItemWeight
{
    public Define.item_type type;
    public string name;
    public int weight;
}
