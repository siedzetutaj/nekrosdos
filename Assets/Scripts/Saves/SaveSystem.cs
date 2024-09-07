using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public static class SaveSystem 
{
    private static string path = Application.persistentDataPath + "/save.json";
    public static void SaveData()
    {
        //BinaryFormatter formatter = new BinaryFormatter();
        //FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new();
        data.Initialize();

        string saveData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, saveData);
        //formatter.Serialize(stream, data);
        //stream.Close();
    }
    public static SaveData LoadData()
    { 
        if(File.Exists(path))
        {
            //BinaryFormatter formatter = new BinaryFormatter();
            //FileStream stream = new FileStream(path, FileMode .Open);

            //SaveData data = formatter.Deserialize(stream) as SaveData;
            //stream.Close();
            
            string saveData = File.ReadAllText(path);

            return JsonUtility.FromJson<SaveData>(saveData);

        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

}
