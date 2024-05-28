using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public List<ConnectedThoughtsGuid> connections = new List<ConnectedThoughtsGuid>();
    public Dictionary<SerializableGuid, ConnectedNode> nodes = new Dictionary<SerializableGuid, ConnectedNode>();

    // Dodaje nowy node
    public void AddNode(SerializableGuid nodeId, SerializableGuid objectId)
    {
        if (!nodes.ContainsKey(nodeId))
        {
            nodes[nodeId] = new ConnectedNode(nodeId, objectId);
        }
    }

    // Dodaje nowe po³¹czenie miêdzy dwoma nodami
    public void AddConnection(SerializableGuid nodeId1, SerializableGuid nodeId2)
    {
        if (nodes.ContainsKey(nodeId1) && nodes.ContainsKey(nodeId2) && !ConnectionExists(nodeId1, nodeId2))
        {
            connections.Add(new ConnectedThoughtsGuid(nodeId1, nodeId2));
        }
    }

    // Usuwa po³¹czenie miêdzy dwoma nodami
    public void RemoveConnection(SerializableGuid nodeId1, SerializableGuid nodeId2)
    {
        connections.RemoveAll(c =>
            (c.Id1 == nodeId1 && c.Id2 == nodeId2) || (c.Id1 == nodeId2 && c.Id2 == nodeId1));
    }

    // Sprawdza, czy po³¹czenie ju¿ istnieje
    private bool ConnectionExists(SerializableGuid nodeId1, SerializableGuid nodeId2)
    {
        return connections.Exists(c =>
            (c.Id1 == nodeId1 && c.Id2 == nodeId2) || (c.Id1 == nodeId2 && c.Id2 == nodeId1));
    }

    // Znajduje grupy po usuniêciu po³¹czenia
    public List<List<ConnectedThoughtsGuid>> FindGroups()
    {
        Dictionary<SerializableGuid, HashSet<SerializableGuid>> adjacencyList = new Dictionary<SerializableGuid, HashSet<SerializableGuid>>();

        // Tworzenie listy s¹siedztwa
        foreach (var connection in connections)
        {
            if (!adjacencyList.ContainsKey(connection.Id1))
                adjacencyList[connection.Id1] = new HashSet<SerializableGuid>();
            if (!adjacencyList.ContainsKey(connection.Id2))
                adjacencyList[connection.Id2] = new HashSet<SerializableGuid>();

            adjacencyList[connection.Id1].Add(connection.Id2);
            adjacencyList[connection.Id2].Add(connection.Id1);
        }

        // Szukanie komponentów spójnych
        List<List<ConnectedThoughtsGuid>> groups = new List<List<ConnectedThoughtsGuid>>();
        HashSet<SerializableGuid> visited = new HashSet<SerializableGuid>();

        foreach (var node in adjacencyList.Keys)
        {
            if (!visited.Contains(node))
            {
                List<SerializableGuid> groupNodes = new List<SerializableGuid>();
                DFS(node, adjacencyList, visited, groupNodes);

                List<ConnectedThoughtsGuid> groupConnections = new List<ConnectedThoughtsGuid>();
                for (int i = 0; i < groupNodes.Count; i++)
                {
                    for (int j = i + 1; j < groupNodes.Count; j++)
                    {
                        if (connections.Exists(c =>
                            (c.Id1 == groupNodes[i] && c.Id2 == groupNodes[j]) ||
                            (c.Id1 == groupNodes[j] && c.Id2 == groupNodes[i])))
                        {
                            groupConnections.Add(new ConnectedThoughtsGuid(groupNodes[i], groupNodes[j]));
                        }
                    }
                }

                groups.Add(groupConnections);
            }
        }

        return groups;
    }

    private void DFS(SerializableGuid node, Dictionary<SerializableGuid, HashSet<SerializableGuid>> adjacencyList, HashSet<SerializableGuid> visited, List<SerializableGuid> groupNodes)
    {
        visited.Add(node);
        groupNodes.Add(node);

        foreach (var neighbor in adjacencyList[node])
        {
            if (!visited.Contains(neighbor))
            {
                DFS(neighbor, adjacencyList, visited, groupNodes);
            }
        }
    }
}
