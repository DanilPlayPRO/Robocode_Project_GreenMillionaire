using UnityEngine;
using System.IO;

public static class SaveSystem
{
    #region Fields
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    #endregion

    #region Save and Load Methods
    public static void SaveData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadData(PlayerData data)
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл збереження не знайдений. Використовуються значення за замовчуванням.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        JsonUtility.FromJsonOverwrite(json, data);
    }
    #endregion
}