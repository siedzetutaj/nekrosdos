using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;

namespace DS.Windows
{
    using Data.Error;
    using Data.Save;
    using Elements;
    using System.Linq;
    using UnityEditor;
    using Utilities;

    public class DSGraphView : GraphView
    {
        private DSEditorWindow editorWindow;
        private DSSearchWindow searchWindow;

        private MiniMap miniMap;
        private Blackboard blackboard;
        private ColorField colorHue;

        private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, DSGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;

        public SerializableDictionary<string, List<string>> deletedGroupedNodeNames;
        public List<string> deletedUngroupedNodeNames;

        public List<DSExposedProperty> exposedProperties;
        public  UnityAction<string,string> OnExposedPropertiesListChange;
        public  UnityAction<string> OnExposedPropertiesListAdd;

        private int nameErrorsAmount;
        private Color selectedColor = Color.white;
        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }

            set
            {
                nameErrorsAmount = value;

                if (nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        public DSGraphView(DSEditorWindow dsEditorWindow)
        {
            editorWindow = dsEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
            groups = new SerializableDictionary<string, DSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();

            deletedGroupedNodeNames = new SerializableDictionary<string, List<string>>();
            deletedUngroupedNodeNames = new List<string>();

            exposedProperties = new List<DSExposedProperty>();

            AddManipulators();
            AddGridBackground();
            AddSearchWindow();
            AddMiniMap();
            AddBlackboard();
            AddColorHue();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
        #region Creations
        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }
        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }
        //private IManipulator CreateRectangleContextualMenu()
        //{
        //    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
        //        menuEvent => menuEvent.menu.AppendAction("Add Rectangle", actionEvent => CreateRectangle( GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
        //    );

        //    return contextualMenuManipulator;
        //}
        //public DSRectangle CreateRectangle(Vector2 position)
        //{
        //    DSRectangle rect = new DSRectangle(position);
        //    AddElement(rect);
        //    return rect;
        //}
        public DSGroup CreateGroup(string title, Vector2 position, bool isLoaded = false)
        {
            DSGroup group = new DSGroup(title, position);

            if (isLoaded )
            {
                group.WasModified = false;
            }

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is DSNode))
                {
                    continue;
                }

                DSNode node = (DSNode) selectedElement;

                group.AddElement(node);
            }

            return group;
        }
        public DSNode CreateNode(string nodeName, DSDialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");

            DSNode node = (DSNode) Activator.CreateInstance(nodeType);

            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }
        #endregion
        #region OnChange
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DSGroup);
                Type edgeType = typeof(Edge);

                List<DSGroup> groupsToDelete = new List<DSGroup>();
                List<DSNode> nodesToDelete = new List<DSNode>();
                List<Edge> edgesToDelete = new List<Edge>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is DSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge) selectedElement;

                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if (selectedElement.GetType() != groupType)
                    {
                        continue;
                    }

                    DSGroup group = (DSGroup) selectedElement;

                    groupsToDelete.Add(group);
                }

                foreach (DSGroup groupToDelete in groupsToDelete)
                {
                    List<DSNode> groupNodes = new List<DSNode>();

                    foreach (GraphElement groupElement in groupToDelete.containedElements)
                    {
                        if (!(groupElement is DSNode))
                        {
                            continue;
                        }

                        DSNode groupNode = (DSNode) groupElement;

                        deletedGroupedNodeNames.AddItem(groupNode.Group.title, groupNode.DialogueName);

                        groupNodes.Add(groupNode);
                    }
                    groupToDelete.RemoveElements(groupNodes);

                    RemoveGroup(groupToDelete);

                    RemoveElement(groupToDelete);
                }

                DeleteElements(edgesToDelete);

                foreach (DSNode nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete.Group != null)
                    {
                        nodeToDelete.Group.RemoveElement(nodeToDelete);
                    }

                    deletedUngroupedNodeNames.Add(nodeToDelete.DialogueName);

                    RemoveUngroupedNode(nodeToDelete);

                    nodeToDelete.DisconnectAllPorts();

                    RemoveElement(nodeToDelete);
                }
            };
        }
        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup) group;
                    DSNode node = (DSNode) element;

                    dsGroup.WasModified = true;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, dsGroup);
                }
            };
        }
        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup) group;
                    DSNode node = (DSNode) element;

                    dsGroup.WasModified = true;

                    RemoveGroupedNode(node, dsGroup);
                    AddUngroupedNode(node);
                }
            };
        }
        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DSGroup dsGroup = (DSGroup) group;

                dsGroup.WasModified = true;

                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        --NameErrorsAmount;
                    }
                }

                RemoveGroup(dsGroup);

                dsGroup.OldTitle = dsGroup.title;

                AddGroup(dsGroup);
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
                        DSNode inputNode = (DSNode) edge.input.node;
                        inputNode.WasModified = true;

                        DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;

                        choiceData.NodeID = inputNode.ID;

                        DSNode outputNode = (DSNode) edge.output.node;
                        outputNode.WasModified = true;
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

                        Edge edge = (Edge) element;

                        DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;

                        choiceData.NodeID = "";
                        
                        DSNode inputNode = (DSNode)edge.input.node;
                        if (inputNode != null)
                        {
                            inputNode.WasModified = true;
                        }

                        DSNode outputNode = (DSNode)edge.output.node;
                        if (outputNode != null)
                        {
                            outputNode.WasModified = true;
                        }
                    }
                }

                return changes;
            };
        }
        #endregion
        #region Additions
        private void AddManipulators()
        {
            SetupZoom(0.001f, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DSDialogueType.MultipleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (If One True)", DSDialogueType.IfOneTrue));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (If All True)", DSDialogueType.IfAllTrue));

            this.AddManipulator(CreateGroupContextualMenu());
            //this.AddManipulator(CreateRectangleContextualMenu());
        }
        public void AddUngroupedNode(DSNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        private void AddGroup(DSGroup group)
        {
            string groupName = group.title.ToLower();

            if (!groups.ContainsKey(groupName))
            {
                DSGroupErrorData groupErrorData = new DSGroupErrorData();

                groupErrorData.Groups.Add(group);

                groups.Add(groupName, groupErrorData);

                return;
            }

            List<DSGroup> groupsList = groups[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.Color;

            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++NameErrorsAmount;

                groupsList[0].SetErrorStyle(errorColor);
            }
        }
        public void AddGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                groupedNodesList[0].SetErrorStyle(errorColor);
            }
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
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
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
        private void AddBlackboard()
        {
            blackboard = new Blackboard(this)
            {
                title = "properties",
                addItemRequested = _blackboard =>
                {
                    AddPropertyToBlackboard(new DSExposedProperty());
                },
                editTextRequested = (blackboard1, element, newValue) =>
                {
                    var oldPropertyName = ((BlackboardField)element).text;
                    if (exposedProperties.Any(x => x.Name == newValue))
                    {
                        EditorUtility.DisplayDialog("Error", "This property name already exist", "OK");
                        return;
                    }

                    var propertyIndex = exposedProperties.FindIndex(x => x.Name == oldPropertyName);
                    exposedProperties[propertyIndex].Name = newValue;
                    ((BlackboardField)element).text = newValue;
                    OnExposedPropertiesListChange?.Invoke(newValue, oldPropertyName);
                },
                scrollable = true,
                visible = false, 
            };
            blackboard.SetPosition(new Rect(5, 50, 250, 600));
            Add(blackboard);
        }
        private void AddColorHue()
        {
            colorHue = new ColorField()
            {
                value = selectedColor,
                visible = false,
            };
            colorHue.style.position = Position.Absolute;
            colorHue.style.bottom = 5; 
            colorHue.style.right = 5;  
            colorHue.style.width = 75; 
            colorHue.style.height = 14; 
            colorHue.RegisterValueChangedCallback(evt =>
            {
                selectedColor = evt.newValue;
            });
            Add(colorHue);
        }
        public void AddPropertyToBlackboard(DSExposedProperty exposedProperty)
        {
            var localPropertyName = "New Bool";
            var localPropertyValue = exposedProperty.Value;

            if (exposedProperty.Name != null)
            {
                localPropertyName = exposedProperty.Name;
            }

            while (exposedProperties.Any(x => x.Name == localPropertyName))
                localPropertyName = $"{localPropertyName}(1)";

            var property = new DSExposedProperty();
            property.Name = localPropertyName;
            property.OldName = localPropertyName;
            property.Value = localPropertyValue;
            exposedProperties.Add(property);

            var container = new VisualElement();
            var blackboardField = new BlackboardField { text = localPropertyName, typeText = "bool property" };
            container.Add(blackboardField);

            var propertyValue = new Toggle("Value: ")
            {
                value = localPropertyValue
            };
            propertyValue.RegisterValueChangedCallback(value =>
            {
                var changingPropertyIndex = exposedProperties.FindIndex(x => x.Name == property.Name);
                exposedProperties[changingPropertyIndex].Value = value.newValue;
            });

            var blackboardValueRow = new BlackboardRow(blackboardField, propertyValue);
            container.Add(blackboardValueRow);
            blackboard.Add(container);

            // Add right-click event handler
            blackboardField.RegisterCallback<ContextualMenuPopulateEvent>(evt =>
            {
                evt.menu.AppendAction("Destroy", action =>
                {
                    RemovePropertyFromBlackboard(property, container);
                });
            });

            OnExposedPropertiesListAdd?.Invoke(localPropertyName);
        }
        private void RemovePropertyFromBlackboard(DSExposedProperty property, VisualElement container)
        {
            exposedProperties.Remove(property);
            blackboard.Remove(container);
            OnExposedPropertiesListChange?.Invoke(property.Name, null);
        }
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
        public void RemoveUngroupedNode(DSNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);

            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                ungroupedNodesList[0].ResetStyle();

                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }
        private void RemoveGroup(DSGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<DSGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);

            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --NameErrorsAmount;

                groupsList[0].ResetStyle();

                return;
            }

            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }
        public void RemoveGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = null;

            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
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

            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();
            exposedProperties.Clear();
            blackboard.Clear();

            NameErrorsAmount = 0;
        }
        #endregion
        #region Togglers
        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }  
        public void ToggleBlackboard()
        {
            blackboard.visible = !blackboard.visible;
        }   
        public void ToggleColorHue()
        {
            colorHue.visible = !colorHue.visible;
        }
        #endregion
        #region Shortcuts
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.ctrlKey)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Q:

                        if (selection.Count == 0)
                        {
                            return;
                        }
                        foreach (var item in selection)
                        {
                            if (item is DSNode node)
                            {

                                if (Color.red != node.mainContainer.style.backgroundColor)
                                {
                                    node.backgroundColor = selectedColor;
                                    node.ResetStyle();
                                }
                            }
                        }
                        break;
                        
                    default: break;
                }
            }
            //// Example: Handle Ctrl+Z (Undo)
            //if (evt.ctrlKey && evt.keyCode == KeyCode.Z)
            //{
            //    Debug.Log("Custom Ctrl+Z in GraphView");
            //    evt.StopImmediatePropagation(); // Prevent further propagation to Unity's global shortcut system
            //    Undo.PerformUndo(); // Perform your custom undo operation or fallback to Unity's undo
            //}
            //// Example: Handle Ctrl+V (Paste)
            //else if (evt.ctrlKey && evt.keyCode == KeyCode.V)
            //{
            //    Debug.Log("Custom Ctrl+V in GraphView");
            //    evt.StopImmediatePropagation(); // Prevent further propagation
            //                                    // Perform your custom paste logic here
            //}
        }
        #endregion
    }
}