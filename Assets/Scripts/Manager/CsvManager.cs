using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvManager
{
    private string path = "ItemWeight";

    public List<ItemWeight> ReadCsv()
    {
        List<ItemWeight> weight_dic = new List<ItemWeight>();
        TextAsset csv = Resources.Load<TextAsset>(path);
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
}

public struct ItemWeight
{
    public Define.item_type type;
    public string name;
    public int weight;
}
