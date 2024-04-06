using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPGraphSaveDataSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public List<TPNodeSaveData> Nodes { get; set; }

    public void Initialize(string fileName)
    {
        FileName = fileName;

        Nodes = new List<TPNodeSaveData>();
    }
}