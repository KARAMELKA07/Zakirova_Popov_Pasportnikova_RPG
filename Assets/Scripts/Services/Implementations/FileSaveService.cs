using System.IO;
using UnityEngine;

public class FileSaveService : ISaveService
{
    private readonly string savePath = Application.persistentDataPath + "/save.json";

    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public GameData LoadGame()
    {
        if (!File.Exists(savePath)) return null;
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameData>(json);
    }
}