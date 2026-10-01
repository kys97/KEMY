using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class JsonManager
{ 
    public void SaveJson(DataClass savedata, string uid)
    {
        Directory.CreateDirectory(Application.persistentDataPath);
        File.WriteAllText(GetAccountPath(uid), JsonUtility.ToJson(savedata, true), Encoding.UTF8);
    }

    public DataClass LoadSaveData(string uid)
    {
        string dataPath = GetAccountPath(uid);
        if (File.Exists(dataPath))
        {
            DataClass gameData = JsonUtility.FromJson<DataClass>(File.ReadAllText(dataPath, Encoding.UTF8));
            if (gameData != null && gameData.info != null && gameData.info.uid == uid) return gameData;
        }

        string legacyPath = Path.Combine(Application.persistentDataPath, "GameData.json");
        if (File.Exists(legacyPath))
        {
            DataClass legacy = JsonUtility.FromJson<DataClass>(File.ReadAllText(legacyPath, Encoding.UTF8));
            if (legacy != null && legacy.info != null && legacy.info.uid == uid) return legacy;
        }
        return new DataClass();
    }

    private static string GetAccountPath(string uid)
    {
        if (string.IsNullOrEmpty(uid)) throw new System.ArgumentException("A Firebase uid is required.", nameof(uid));
        return Path.Combine(Application.persistentDataPath, "GameData_" + uid + ".json");
    }
}
