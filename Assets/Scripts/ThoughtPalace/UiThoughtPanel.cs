using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiThoughtPanel : MonoBehaviourSingleton<UiThoughtPanel>
{
    [NonSerialized] public bool isDraggingThought;
    [NonSerialized] public bool isCreatingLine = false;
    [NonSerialized] public LineController activeLineController;
    [NonSerialized] public InformationController firstThoughToConnect; //one that currently creates line
    [NonSerialized] public SerializableGuid FistID;

    [Header("Set up")]
    [SerializeField] public UIInformationDisplay informationDisplay;
    [SerializeField] public TextMeshProUGUI descriptionTMP;
    [SerializeField] public RectTransform ThoughtHolderTransform;
    [SerializeField] public RectTransform LineHolder;
    [SerializeField] public TPAllConnectionsSO ThoughtConnections;
    [SerializeField] public GameObject linePrefab;
    //[SerializeField] public List<ConnectionList> PlayerThoughtConnections = new();
    [SerializeField] public Transform draggedParent;
    [SerializeField] private GameObject _thoughtPalaceContainer;
    [SerializeField] private GameObject _thoughtPrefab;


    [Header("Raycast to line")]
    public GameObject dotPrefab; // Prefabrykat kropki
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private LayerMask _uiLineLayerMask;
    [SerializeField] private InputSystem _inputSystem;
    private bool _hasClicked = false;

    [Header("Debug")]
    public List<ConnectionList> connections = new();
    public SerializableDictionary<SerializableGuid, ConnectedNode> nodes = new SerializableDictionary<SerializableGuid, ConnectedNode>();
    public SerializableDictionary<SerializableGuid,InformationController> allCreatedThoughts = new();

    public void Initialize()
    {
        _uiCamera = Camera.main;

        _inputSystem = InputSystem.Instance;
        OnEnable();
        _thoughtPalaceContainer.SetActive(false);
    }
    private void OnEnable()
    {
        if (_inputSystem)
        {
            _inputSystem.onTPLeftClickDown += OnMouseLeftClickDown;
            _inputSystem.onTPLeftClickUp += OnMouseLeftClickUp;

            _inputSystem.onTPRightClickDown += OnMouseRightClickDown;
            _inputSystem.onTPRightClickUp += OnMouseRightClickUp;  

            _inputSystem.onTPMiddleClickDown += OnMouseMiddleClickDown;
            _inputSystem.onTPMiddleClickUp += OnMouseMiddleClickUp;
        }
    }
    private void OnDisable()
    {
        _inputSystem.onTPLeftClickDown -= OnMouseLeftClickDown;
        _inputSystem.onTPLeftClickUp -= OnMouseLeftClickUp;

        _inputSystem.onTPRightClickDown -= OnMouseRightClickDown;
        _inputSystem.onTPRightClickUp -= OnMouseRightClickUp;

        _inputSystem.onTPMiddleClickDown -= OnMouseMiddleClickDown;
        _inputSystem.onTPMiddleClickUp -= OnMouseMiddleClickUp;
    }
    private void Start()
    {
        ThoughtConnections = TPAllConnectionsSO.instance;
        ThoughtConnections.AllConnections = ThoughtConnections.AllConnections.Where(item => item != null).ToList();
    }
    #region Operations on thoughts and connections
    public void AddNode(SerializableGuid nodeId, SerializableGuid objectId)
    {
        if (!nodes.ContainsKey(nodeId))
        {
            nodes[nodeId] = new ConnectedNode(nodeId, objectId);
        }
    }
    public void AddConnection(SerializableGuid nodeId2, Vector2 anchoredPos, InformationController thought)
    {
        activeLineController.IsDraggedByMouse = false;
        activeLineController.ChangePointPosition(1, anchoredPos);
        activeLineController.connectionGuids.Id1 = FistID;
        activeLineController.connectionGuids.Id2 = nodeId2;

        if (LineManager.Instance.CheckIfLineExist(activeLineController))
        {
            thought.LineRenderers.Remove(activeLineController);
            CancelDrawingLine(true);
            return;
        }
        LineManager.Instance.addLineController(activeLineController);
        Grouping(FistID, nodeId2);


        activeLineController = null;
        firstThoughToConnect = null;
        isCreatingLine = false;
    }
    public void LoadConnections(List<ConnectedThoughtsGuid> lineConnectionsGuids)
    {
        foreach(LineController lineControllerToRemove in LineManager.Instance.lineControllers)
        {
            Destroy(lineControllerToRemove.gameObject);
        }
        LineManager.Instance.lineControllers.Clear();
        foreach(ConnectedThoughtsGuid lineConnectionGuids in lineConnectionsGuids)
        {
            InformationController firstThought = allCreatedThoughts[lineConnectionGuids.Id1];
            InformationController secondThought = allCreatedThoughts[lineConnectionGuids.Id2];
            
            GameObject line = Instantiate(linePrefab, LineHolder);

            LineController lineController = line.GetComponent<LineController>();
            lineController.Awake();
            lineController.LoadSetup(lineConnectionGuids);
            LineManager.Instance.addLineController(lineController);
            firstThought.LineRenderers.Add(lineController, true);
            secondThought.LineRenderers.Add(lineController, false);
            Grouping(lineConnectionGuids.Id1,lineConnectionGuids.Id2);
            lineController.ChangePointPosition(0, firstThought.recTransform.anchoredPosition);
            lineController.ChangePointPosition(1, secondThought.recTransform.anchoredPosition);

        }
    }
    private void Grouping(SerializableGuid nodeId1, SerializableGuid nodeId2)
    {

        if (nodes.ContainsKey(nodeId1) && nodes.ContainsKey(nodeId2) && !ConnectionExists(nodeId1, nodeId2))
        {
            var newConnection = new ConnectedThoughtsGuid(nodeId1, nodeId2);

            // Szukamy, do jakich grup nale¿¹ te nody
            var group1 = FindGroup(nodeId1);
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
        LineManager.Instance.removeLineController(line);
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

        foreach (var thought in allCreatedThoughts)
        {
            if (thought.Value.ThoughtNodeGuid == connectionToRemove.Id1 || thought.Value.ThoughtNodeGuid == connectionToRemove.Id2)
            {
                thought.Value.LineRenderers.Remove(line);
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
    private void CancelDrawingLine(bool isduplicate = false)
    {
        FistID = SerializableGuid.Empty;
        var lineController = activeLineController;
        activeLineController = null;
        lineController.IsDraggedByMouse = false;
        //to usuwa w innej mysli nie tej ktora zaczyna, ten skrypt ejst przypisywany do kazdej mysli a nie jest globalny
        firstThoughToConnect.LineRenderers.Remove(lineController);
        Destroy(lineController.gameObject);
        isCreatingLine = false;
        firstThoughToConnect = null;
        
        
        if (isduplicate)
        {

        }
    }
    #endregion
    #region Other
    private void OnMouseLeftClickDown()
    {
        if (isCreatingLine && !_hasClicked)
        {
            CancelDrawingLine();
        }
    }
    private void OnMouseLeftClickUp()
    {
        _hasClicked = false;
    }
    private void OnMouseRightClickDown()
    {

    }
    private void OnMouseRightClickUp()
    {

    }
    private void OnMouseMiddleClickDown()
    {
        if (!isDraggingThought && !_hasClicked)
        {
            var line = DetectLine();
            if (line)
            {
                DestoryLine(line);
            }
            _hasClicked = true;
        }
    }
    private void OnMouseMiddleClickUp()
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
    public void DeleateThought(InformationController thought)
    {
        var nulls = thought.LineRenderers.Keys.Where(x => x == null);
        foreach (var item in nulls)
        {
            thought.LineRenderers.Remove(item);
        }
        foreach (var connection in thought.LineRenderers.Keys.ToList())
        {
            DestoryLine(connection);
        }
        allCreatedThoughts.Remove(thought.ThoughtNodeGuid);
        Destroy(thought.gameObject);
    }
    private void DestoryLine(LineController line)
    {
        RemoveConnection(line);
        Destroy(line.gameObject);
    }
    public void LoadThoughts(List<ThoughtSaveData> data)
    {
        ResetData();
        foreach (ThoughtSaveData thoughtSaveData in data)
        {
            GameObject thoughtObject = Instantiate(_thoughtPrefab, ThoughtHolderTransform);
            thoughtObject.GetComponent<InformationPrefabData>().Initialize(thoughtSaveData.ThoughtSO, descriptionTMP, draggedParent, this, informationDisplay, _uiCamera); 
            thoughtObject.GetComponent<InformationController>().LoadThought(thoughtSaveData);
        }
    }
    private void ResetData()
    {
        //Destroying thoughts gameobjects and clearing lists
        foreach(var thought in allCreatedThoughts)
        {
            Destroy(thought.Value.gameObject);
        }
        allCreatedThoughts.Clear();
        nodes.Clear();
        connections.Clear();
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