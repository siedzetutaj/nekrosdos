using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float[] _playerPosition = new float[3];
    public List<TPThoughtSO> unlockedThoughtsSO = new();
    public SaveData()
    {
        var position = PlayerController.Instance.gameObject.transform.position;
        _playerPosition[0] = position.x;
        _playerPosition[1] = position.y;
        _playerPosition[2] = position.z;
        unlockedThoughtsSO = UIInformationDisplay.Instance.AllUnlockedThoughts.Select(infoPrefabData => infoPrefabData.MyThought).ToList();
    }
}