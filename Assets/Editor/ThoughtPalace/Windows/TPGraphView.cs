using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Linq;

public enum TPThoughtType
{
    Information,
}
public class TPGraphView : GraphView
{
    private TPEditorWindow editorWindow;
    private TPSearchWindow searchWindow;

    private MiniMap miniMap;
    // private Blackboard blackboard;

    //public List<TPExposedProperty> exposedProperties;
    //public UnityAction<string,string> OnExposedPropertiesListChange;
    //public UnityAction<string> OnExposedPropertiesListAdd;

    public TPGraphView(TPEditorWindow dsEditorWindow)
    {
        editorWindow = dsEditorWindow;

        //exposedProperties = new List<DSExposedProperty>();

        AddManipulators();
        AddGridBackground();
        AddSearchWindow();
        AddMiniMap();
        // AddBlackboard();

        OnElementsDeleted();
        OnGraphViewChanged();

        AddStyles();
        AddMiniMapStyles();
    }



    #region Creations
    private IManipulator CreateNodeContextualMenu(string actionTitle, TPThoughtType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }

    public TPNode CreateNode(string nodeName, TPThoughtType dialogueType, Vector2 position, bool shouldDraw = true)
    {
        //Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
        Type nodeType = Type.GetType($"TP{dialogueType}Node");

        TPNode node = (TPNode)Activator.CreateInstance(nodeType);

        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

        AddNode(node);

        return node;
    }
    #endregion
    #region OnChange
    private void OnElementsDeleted()
    {
        deleteSelection = (operationName, askUser) =>
        {
            Type edgeType = typeof(Edge);

            List<TPNode> nodesToDelete = new List<TPNode>();
            List<Edge> edgesToDelete = new List<Edge>();

            foreach (GraphElement selectedElement in selection)
            {
                if (selectedElement is TPNode node)
                {
                    nodesToDelete.Add(node);

                    continue;
                }

                if (selectedElement.GetType() == edgeType)
                {
                    Edge edge = (Edge)selectedElement;

                    edgesToDelete.Add(edge);

                    continue;
                }
            }
            DeleteElements(edgesToDelete);

            foreach (TPNode nodeToDelete in nodesToDelete)
            {
                RemoveNode(nodeToDelete);

                nodeToDelete.DisconnectAllPorts();

                RemoveElement(nodeToDelete);
            }
        };
    }
    private void OnGraphViewChanged()
    {
        graphViewChanged = (changes) =>
        {
            if (changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    TPNode nextNode = (TPNode)edge.input.node;

                    List<TPNextThoughtSaveData> saveDataList = (List<TPNextThoughtSaveData>)edge.output.userData;
                    TPNextThoughtSaveData saveData = new TPNextThoughtSaveData() { NodeID = nextNode.ID };
                    saveDataList.Add(saveData);
                }
            }

            if (changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                    {
                        continue;
                    }

                    Edge edge = (Edge)element;

                    TPNode nodeDataToRemove = (TPNode)edge.input.node;

                    List<TPNextThoughtSaveData> saveDataList = (List<TPNextThoughtSaveData>)edge.output.userData;

                    TPNextThoughtSaveData saveDataToRemove = saveDataList.Find(x => x.NodeID == nodeDataToRemove.ID);

                    saveDataList.Remove(saveDataToRemove);
                }
            }

            return changes;
        };
    }
    #endregion
    #region Additions
    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(CreateNodeContextualMenu("Add Node (Information)", TPThoughtType.Information));
    }
    public void AddNode(TPNode node)
    {
        // mozna dodac efekt kiedy dodawany jest nowy node
    }
    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);
    }
    private void AddSearchWindow()
    {
        if (searchWindow == null)
        {
            searchWindow = ScriptableObject.CreateInstance<TPSearchWindow>();
        }

        searchWindow.Initialize(this);

        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }
    private void AddMiniMap()
    {
        miniMap = new MiniMap()
        {
            anchored = true
        };

        miniMap.SetPosition(new Rect(15, 50, 200, 180));

        Add(miniMap);

        miniMap.visible = false;
    }
    //private void AddBlackboard()
    //{
    //    blackboard = new Blackboard(this)
    //    {
    //        title = "properties",
    //        addItemRequested = _blackboard =>
    //        {
    //            AddPropeprtyToBlackBoard(new DSExposedProperty());
    //        },
    //        editTextRequested = (blackboard1, element, newValue) =>
    //        {
    //            var oldPropertyName = ((BlackboardField)element).text;
    //            if (exposedProperties.Any(x => x.Name == newValue))
    //            {
    //                EditorUtility.DisplayDialog("Error", "This property name already exist", "OK");
    //                return;
    //            }

    //            var propertyIndex = exposedProperties.FindIndex(x => x.Name == oldPropertyName);
    //            exposedProperties[propertyIndex].Name = newValue;
    //            ((BlackboardField)element).text = newValue;
    //            OnExposedPropertiesListChange?.Invoke(newValue, oldPropertyName);
    //        },
    //        scrollable = true,
    //    };
    //    blackboard.SetPosition(new Rect(5, 50, 250, 600));
    //    Add(blackboard);
    //}
    //public void AddPropeprtyToBlackBoard(DSExposedProperty exposedProperty)
    //{
    //    var localPropertyName = "New Bool";
    //    var localPropertyValue = exposedProperty.Value;
    //    if (exposedProperty.Name != null)
    //    {
    //        localPropertyName = exposedProperty.Name;
    //    }
    //    while (exposedProperties.Any(x => x.Name == localPropertyName))
    //        localPropertyName = $"{localPropertyName}(1)";

    //    var property = new DSExposedProperty();
    //    property.Name = localPropertyName;
    //    property.OldName = localPropertyName;
    //    property.Value = localPropertyValue;
    //    exposedProperties.Add(property);

    //    var container = new VisualElement();
    //    var blackboardField = new BlackboardField {text = localPropertyName, typeText = "bool property"};
    //    container.Add(blackboardField);

    //    var propertyValue = new Toggle("Value: ")
    //    {
    //        value = localPropertyValue
    //    };
    //    propertyValue.RegisterValueChangedCallback(value =>
    //    {
    //        var changingPropertyIndex = exposedProperties.FindIndex(x => x.Name == property.Name);
    //        exposedProperties[changingPropertyIndex].Value = value.newValue;
    //    });
    //    var blackboardValueRow = new BlackboardRow(blackboardField, propertyValue);
    //    container.Add(blackboardValueRow);
    //    blackboard.Add(container);
    //    OnExposedPropertiesListAdd?.Invoke(localPropertyName);
    //}
    private void AddStyles()
    {
        this.AddStyleSheets(
            "DialogueSystem/DSGraphViewStyles.uss",
            "DialogueSystem/DSNodeStyles.uss"
        );
    }
    private void AddMiniMapStyles()
    {
        StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
        StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

        miniMap.style.backgroundColor = backgroundColor;
        miniMap.style.borderTopColor = borderColor;
        miniMap.style.borderRightColor = borderColor;
        miniMap.style.borderBottomColor = borderColor;
        miniMap.style.borderLeftColor = borderColor;
    }

    #endregion
    #region Removals
    public void RemoveNode(TPNode node)
    {
        //Stanie siê coœ kiedy usunie siê noda
    }
    #endregion
    #region Getters
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort == port)
            {
                return;
            }

            if (startPort.node == port.node)
            {
                return;
            }

            if (startPort.direction == port.direction)
            {
                return;
            }

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        Vector2 worldMousePosition = mousePosition;

        if (isSearchWindow)
        {
            worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
        }

        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

        return localMousePosition;
    }
    #endregion
    #region Utilities
    public void ClearGraph()
    {
        graphElements.ForEach(graphElement => RemoveElement(graphElement));

        // blackboard.Clear();
    }
    #endregion
    #region Togglers
    public void ToggleMiniMap()
    {
        miniMap.visible = !miniMap.visible;
    }
    //public void ToggleBlackboard()
    //{
    //    blackboard.visible = !blackboard.visible;
    //}
    #endregion
}
