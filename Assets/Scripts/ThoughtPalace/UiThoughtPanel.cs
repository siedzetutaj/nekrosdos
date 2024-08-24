using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class UiThoughtPanel : MonoBehaviourSingleton<UiThoughtPanel>
{
    [NonSerialized] public bool isDraggingThought;
    [NonSerialized] public bool isCreatingLine = false;
    [NonSerialized] public LineController activeLineController;
    [NonSerialized] public GameObject activeThough; //one that currently creates line
    [NonSerialized] public SerializableGuid FistID;

    [Header("Set up")]
    [SerializeField] public TextMeshProUGUI descriptionTMP;
    [SerializeField] public RectTransform ThoughtPanelTransform;
    [SerializeField] public RectTransform LineHolder;
    [SerializeField] public TPAllConnectionsSO ThoughtConnections;
    [SerializeField] public List<ConnectionList> PlayerThoughtConnections = new();
    [SerializeField] private GameObject ThoughtPalaceContainer;

    [Header("Raycast to line")]
    public GameObject dotPrefab; // Prefabrykat kropki
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private LayerMask _uiLineLayerMask;
    [SerializeField] private InputSystem _inputSystem;
    private bool hasClicked = false;

    [Header("Debug")]
    [SerializeField] public List<ConnectionList> connections = new();
    public SerializableDictionary<SerializableGuid, ConnectedNode> nodes = new SerializableDictionary<SerializableGuid, ConnectedNode>();
    [SerializeField] public List<InformationController> createdThoughts = new();

    public void Initialize()
    {
        _uiCamera = Camera.main;

        _inputSystem = InputSystem.Instance;
        OnEnable();
        ThoughtPalaceContainer.SetActive(false);
    }
    private void OnEnable()
    {
        if (_inputSystem)
        {
            _inputSystem.onTPLeftClickDown += OnMouseLeftClickDown;
            _inputSystem.onTPLeftClickUp += OnMouseLeftClickUp;
            _inputSystem.onTPRightClickDown += OnMouseRightClickDown;
            _inputSystem.onTPRightClickUp += OnMouseRightClickUp;
        }
    }

    private void OnDisable()
    {
        _inputSystem.onTPLeftClickDown -= OnMouseLeftClickDown;
        _inputSystem.onTPLeftClickUp -= OnMouseLeftClickUp;
        _inputSystem.onTPRightClickDown -= OnMouseRightClickDown;
        _inputSystem.onTPRightClickUp -= OnMouseRightClickUp;

    }
    private void Start()
    {
        ThoughtConnections = TPAllConnectionsSO.instance;
        ThoughtConnections.AllConnections = ThoughtConnections.AllConnections.Where(item => item != null).ToList();
    }
    #region Grouping, Adding and Removing Connected Thoughts
    public void AddNode(SerializableGuid nodeId, SerializableGuid objectId)
    {
        if (!nodes.ContainsKey(nodeId))
        {
            nodes[nodeId] = new ConnectedNode(nodeId, objectId);
        }
    }
    public void AddConnection(SerializableGuid nodeId2)
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
                var newGroup = new ConnectionList { thoughtsList = new List<ConnectedThoughtsGuid> { newConnection } };
                connections.Add(newGroup);
            }
        }
    }
    public void RemoveConnection(LineController line)
    {
        var connectionToRemove = new ConnectedThoughtsGuid(line.connectionGuids.Id1, line.connectionGuids.Id2);
        ConnectionList groupToUpdate = null;

        foreach (var connectionList in connections)
        {
            if (connectionList.thoughtsList.Contains(connectionToRemove))
            {
                groupToUpdate = connectionList;
                break;
            }
        }

        if (groupToUpdate != null)
        {
            groupToUpdate.thoughtsList.Remove(connectionToRemove);

            // Jeœli po usuniêciu po³¹czenia w grupie nie ma ju¿ ¿adnych po³¹czeñ, usuwamy grupê
            if (groupToUpdate.thoughtsList.Count == 0)
            {
                connections.Remove(groupToUpdate);
            }
            else
            {
                // Przebudowujemy grupy po usuniêciu po³¹czenia
                RebuildGroups();
            }
        }

        foreach (var thought in createdThoughts)
        {
            if (thought.ThoughtNodeGuid == connectionToRemove.Id1 || thought.ThoughtNodeGuid == connectionToRemove.Id2)
            {
                thought.LineRenderers.Remove(line);
            }
        }
    }
    private void RebuildGroups()
    {
        // Tworzymy now¹ listê grup
        var newConnections = new List<ConnectionList>();
        var allConnections = new List<ConnectedThoughtsGuid>();

        // Zbieramy wszystkie po³¹czenia
        foreach (var connectionList in connections)
        {
            allConnections.AddRange(connectionList.thoughtsList);
        }

        // Resetujemy aktualne po³¹czenia
        connections.Clear();

        // U¿ywamy BFS do znalezienia wszystkich po³¹czonych komponentów
        var visitedNodes = new HashSet<SerializableGuid>();
        foreach (var node in nodes.Keys)
        {
            if (!visitedNodes.Contains(node))
            {
                var newGroup = new ConnectionList { thoughtsList = new List<ConnectedThoughtsGuid>() };
                var queue = new Queue<SerializableGuid>();
                queue.Enqueue(node);
                visitedNodes.Add(node);

                while (queue.Count > 0)
                {
                    var currentNode = queue.Dequeue();
                    foreach (var connection in allConnections)
                    {
                        if ((connection.Id1 == currentNode || connection.Id2 == currentNode) &&
                            !newGroup.thoughtsList.Contains(connection))
                        {
                            newGroup.thoughtsList.Add(connection);

                            var neighborNode = connection.Id1 == currentNode ? connection.Id2 : connection.Id1;
                            if (!visitedNodes.Contains(neighborNode))
                            {
                                visitedNodes.Add(neighborNode);
                                queue.Enqueue(neighborNode);
                            }
                        }
                    }
                }

                if (newGroup.thoughtsList.Count > 0)
                {
                    newConnections.Add(newGroup);
                }
            }
        }

        connections = newConnections;
    }
    // Pobiera wszystkie po³¹czenia dla danego node
    private List<ConnectedThoughtsGuid> GetConnections(SerializableGuid nodeId)
    {
        var result = new List<ConnectedThoughtsGuid>();
        foreach (var connectionList in connections)
        {
            result.AddRange(connectionList.thoughtsList.FindAll(c => c.Id1 == nodeId || c.Id2 == nodeId));
        }
        return result;
    }
    //// DFS do zbierania po³¹czeñ w grupie
    //private void DFS(SerializableGuid nodeId, HashSet<SerializableGuid> visitedNodes, ConnectionList group, List<ConnectedThoughtsGuid> allConnections)
    //{
    //    visitedNodes.Add(nodeId);
    //    foreach (var connection in allConnections)
    //    {
    //        if (connection.Id1 == nodeId && !visitedNodes.Contains(connection.Id2))
    //        {
    //            group.thoughtsList.Add(connection);
    //            DFS(connection.Id2, visitedNodes, group, allConnections);
    //        }
    //        else if (connection.Id2 == nodeId && !visitedNodes.Contains(connection.Id1))
    //        {
    //            group.thoughtsList.Add(connection);
    //            DFS(connection.Id1, visitedNodes, group, allConnections);
    //        }
    //    }
    //}
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
    // Pobiera listê identyfikatorów nodów w tej samej grupie co dany node
    public List<SerializableGuid> GetNodesInGroup(SerializableGuid nodeId)
    {
        var group = FindGroup(nodeId);
        if (group == null)
        {
            return new List<SerializableGuid>();
        }

        var nodesInGroup = new HashSet<SerializableGuid>();
        foreach (var connection in group.thoughtsList)
        {
            nodesInGroup.Add(connection.Id1);
            nodesInGroup.Add(connection.Id2);
        }

        return new List<SerializableGuid>(nodesInGroup);
    }
    #endregion
    #region Detecting Lines via Raycast
    private void OnMouseLeftClickDown()
    {
        if (!isDraggingThought && !hasClicked)
        {
            var line = DetectLine();
            if (line)
            {
                RemoveConnection(line);
                Destroy(line.gameObject);
            }
            hasClicked = true;
        }
    }
    private void OnMouseLeftClickUp()
    {
        hasClicked = false;
    }
    private void OnMouseRightClickDown()
    {

    }
    private void OnMouseRightClickUp()
    {

    }
    private LineController DetectLine()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Konwertujemy pozycjê myszy z ekranu do œwiata 2D
        Vector2 worldPoint = _uiCamera.ScreenToWorldPoint(mousePosition);

        // Wykonujemy raycast 2D w kierunku pozycji myszy
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, _uiLineLayerMask);
        if (hit)
        {
            hit.collider.gameObject.TryGetComponent(out LineController line);
            return line;
        }
        return null;
    }

    #endregion
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