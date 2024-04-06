using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class TPThoughtConections : ScriptableObject
{
    [SerializeField] List<string> CorrectConections = new List<string>();
    public void AddConnection(string connection)
    {
        CorrectConections.Add(connection);
    } 
    public void RemoveConnection(string connection)
    {
        CorrectConections.Remove(connection);
    }
    public void Reset()
    {
        CorrectConections.Clear();
    }
}
