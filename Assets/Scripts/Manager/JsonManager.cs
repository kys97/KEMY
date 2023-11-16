using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class JsonManager
{
    public void SaveJson(DataClass savedata) // 데이터를 저장하는 함수
    {
        string jsonText;

    #if UNITY_EDITOR_WIN
        string DataPath = Application.dataPath + "/Data/GameData.json";
    #endif

    #if UNITY_ANDROID
        string DataPath = Application.persistentDataPath + "/GameData.json";
    #endif

        jsonText = JsonUtility.ToJson(savedata, true);
        FileStream fileStream = new FileStream(DataPath, FileMode.Create);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();
    }

    public DataClass LoadSaveData()
    {
        DataClass gameData;

    #if UNITY_EDITOR_WIN
        string DataPath = Application.dataPath + "/Data/GameData.json";
    #endif

    #if UNITY_ANDROID
        string DataPath = Application.persistentDataPath + "/GameData.json";
    #endif

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        if (File.Exists(DataPath))
        {
            FileStream stream = new FileStream(DataPath, FileMode.Open);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            string jsonData = Encoding.UTF8.GetString(bytes);
            gameData = JsonUtility.FromJson<DataClass>(jsonData);
        }
        else
        {
            gameData = new DataClass();
        }

        return gameData;
    }
}
