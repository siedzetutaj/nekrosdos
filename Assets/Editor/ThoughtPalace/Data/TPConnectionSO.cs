using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConncetionSO", menuName = "ScriptableObjects/ConncetionSO", order = 1)]
public class TPConnectionSO : ScriptableObject
{
    [field: SerializeField] public string ConnectionIDs { get; set; }
    public void Initialize(string text)
    {
        ConnectionIDs = text;
    }
}