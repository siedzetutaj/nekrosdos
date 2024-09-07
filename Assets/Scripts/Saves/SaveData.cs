using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    //Player
    public float[] _playerPosition = new float[3];
    
    //Informations
    public List<TPThoughtSO> unlockedInformationsSO = new();

    //Thoughts
    public List<ThoughtSaveData> thoughts = new();
    
    //Lines
    public List<ConnectedThoughtsGuid> lineConnectionGuids = new();

    //Other
    public string currentSceneName;
    public void Initialize()
    {
        //Player
        var position = PlayerController.Instance.gameObject.transform.position;
        _playerPosition[0] = position.x;
        _playerPosition[1] = position.y;
        _playerPosition[2] = position.z;
        
        //Informations
        unlockedInformationsSO = UIInformationDisplay.Instance.AllUnlockedInformations.Select(infoPrefabData => infoPrefabData.MyThought).ToList();

        //Thoughts
        foreach (var thoughtController in UiThoughtPanel.Instance.allCreatedThoughts)
        {
            ThoughtSaveData thoughtData = new ThoughtSaveData(thoughtController.Key, thoughtController.Value.LineRenderers,
                thoughtController.Value.Thought, thoughtController.Value.gameObject.GetComponent<RectTransform>().localPosition);
            thoughts.Add(thoughtData);
        }
        //Linie
        foreach(var lineController in LineManager.Instance.lineControllers)
        {
            lineConnectionGuids.Add(lineController.connectionGuids);
        }
        //Other
        currentSceneName = SceneManager.GetActiveScene().name;
    }
}

[System.Serializable]
public class ThoughtSaveData
{
    public SerializableGuid Guid;
    public SerializableDictionary<LineController,bool> LineRenderers = new();
    public TPThoughtSO ThoughtSO;
    public Vector3 Position;
    public ThoughtSaveData(SerializableGuid guid, SerializableDictionary<LineController,bool> lineRenderes,
        TPThoughtSO thoughtSO, Vector3 positon)
    {
        Guid = guid;
        LineRenderers = lineRenderes;
        ThoughtSO = thoughtSO;
        Position = positon;
    }
}