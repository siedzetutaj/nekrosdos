using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class TPThoughtConections : ScriptableObject
{
    [SerializeField] List<ConnectedThoughtsGuid> CorrectConections = new List<ConnectedThoughtsGuid>();
    public void AddConnection(ConnectedThoughtsGuid connection)
    {
        CorrectConections.Add(connection);
    } 
    public void RemoveConnection(ConnectedThoughtsGuid connection)
    {
        CorrectConections.Remove(connection);
    }
    public void Reset()
    {
        CorrectConections.Clear();
    }
}
