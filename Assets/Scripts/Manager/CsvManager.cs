using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvManager
{
    private string path = "ItemWeight";

    public Dictionary<string, ItemWeight> ReadCsv()
    {
        Dictionary<string, ItemWeight> weight_dic = new Dictionary<string, ItemWeight>();
        TextAsset csv = Resources.Load<TextAsset>(path);
        StringReader reader = new StringReader(csv.text);

        while(reader.Peek() > -1)
        {
            string line = reader.ReadLine();

            var data = line.Split(',');
            var temp = new ItemWeight { type = (Define.item_type)Enum.Parse(typeof(Define.item_type), data[1]), weight = int.Parse(data[2]) };
            weight_dic.Add(data[0], temp);
        }

        return weight_dic;
    }
}

public struct ItemWeight
{
    public Define.item_type type;
    public int weight;
}
