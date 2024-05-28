using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class UiThoughtPanel : MonoBehaviour
{
    [NonSerialized] public bool isBeingDragged;
    [NonSerialized] public bool isCreatingLine = false;
    [NonSerialized] public LineController activeLineController;
    [NonSerialized] public GameObject activeThough; //one that currently creates line
    [NonSerialized] public SerializableGuid FistID; 

    [SerializeField] public TextMeshProUGUI descriptionTMP;
    [SerializeField] public RectTransform ThoughtPanelTransform;
    [SerializeField] public RectTransform LineHolder;
    [SerializeField] public TPAllConnectionsSO ThoughtConnections;
    [SerializeField] public List<ConnectionList> PlayerThoughtConnections = new();

    [SerializeField] public List<ConnectionList> connections = new();
    public SerializableDictionary<SerializableGuid, ConnectedNode> nodes = new SerializableDictionary<SerializableGuid, ConnectedNode>();

    private void Start()
    {
        ThoughtConnections.AllConnections = ThoughtConnections.AllConnections.Where(item => item != null).ToList();
    }
    public void AddNode(SerializableGuid nodeId, SerializableGuid objectId)
    {
        if (!nodes.ContainsKey(nodeId))
        {
            nodes[nodeId] = new ConnectedNode(nodeId, objectId);
        }
    }
    public void AddConnection( SerializableGuid nodeId2)
    {
        if (nodes.ContainsKey(FistID) && nodes.ContainsKey(nodeId2) && !ConnectionExists(FistID, nodeId2))
        {
            var newConnection = new ConnectedThoughtsGuid(FistID, nodeId2);

            // Szukamy, do jakich grup nale¿¹ te nody
            var group1 = FindGroup(FistID);
            var group2 = FindGroup(nodeId2);

            // Jeœli oba nody s¹ w tej samej grupie, dodaj po³¹czenie do tej grupy
            if (group1 != null && group1 == group2)
            {
                group1.thoughtsList.Add(newConnection);
            }
            // Jeœli nody s¹ w ró¿nych grupach, ³¹czymy te grupy
            else if (group1 != null && group2 != null)
            {
                group1.thoughtsList.Add(newConnection);
                group1.thoughtsList.AddRange(group2.thoughtsList);
                connections.Remove(group2);
            }
            // Jeœli tylko jeden node jest w grupie, dodajemy drugi node do tej grupy
            else if (group1 != null)
            {
                group1.thoughtsList.Add(newConnection);
            }
            else if (group2 != null)
            {
                group2.thoughtsList.Add(newConnection);
            }
            // Jeœli ¿aden z nodów nie jest w grupie, tworzymy now¹ grupê
            else
            {
                connections.Add(new ConnectionList { thoughtsList = new List<ConnectedThoughtsGuid> { newConnection } });
            }
        }
    }

    public void RemoveConnection(SerializableGuid nodeId1, SerializableGuid nodeId2)
    {
        foreach (var connectionList in connections)
        {
            connectionList.thoughtsList.RemoveAll(c =>
                (c.Id1 == nodeId1 && c.Id2 == nodeId2) ||
                (c.Id1 == nodeId2 && c.Id2 == nodeId1));

            // Jeœli po usuniêciu po³¹czenia w grupie nie ma ju¿ ¿adnych po³¹czeñ, usuwamy grupê
            if (connectionList.thoughtsList.Count == 0)
            {
                connections.Remove(connectionList);
                break; // konieczne, aby przerwaæ iteracjê po usuniêciu elementu
            }
        }
    }
    private bool ConnectionExists(SerializableGuid nodeId1, SerializableGuid nodeId2)
    {
        foreach (var group in connections)
        {
            if (group.thoughtsList.Exists(c =>
                (c.Id1 == nodeId1 && c.Id2 == nodeId2) ||
                (c.Id1 == nodeId2 && c.Id2 == nodeId1)))
            {
                return true;
            }
        }
        return false;
    }
    private ConnectionList FindGroup(SerializableGuid nodeId)
    {
        foreach (var group in connections)
        {
            foreach (var connection in group.thoughtsList)
            {
                if (connection.Id1 == nodeId || connection.Id2 == nodeId)
                {
                    return group;
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public struct ConnectedNode
{
    [field: SerializeField] public SerializableGuid NodeId { get; private set; }
    [field: SerializeField] public SerializableGuid ObjectId { get; private set; }

    public ConnectedNode(SerializableGuid nodeId, SerializableGuid objectId)
    {
        NodeId = nodeId;
        ObjectId = objectId;
    }
}
[System.Serializable]
public class ConnectionList 
{
    public List<ConnectedThoughtsGuid> thoughtsList;
}