using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Utilities
{
    using Codice.Client.BaseCommands.CheckIn;
    using Data;
    using Data.Save;
    using Elements;
    using Windows;

    public static class DSIOUtility
    {
        private static DSGraphView graphView;

        private static string graphFileName;
        private static string containerFolderPath;

        private static List<DSNode> nodes;
        private static List<DSGroup> groups;

        private static Dictionary<string, DSDialogueGroupSO> createdDialogueGroups;
        private static Dictionary<string, DSDialogueSO> createdDialogues;

        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;

        private static List<DSDialogueSO> addedUngroupedDialogues;


        public static void Initialize(DSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphName}";

            nodes = new List<DSNode>();
            groups = new List<DSGroup>();

            createdDialogueGroups = new Dictionary<string, DSDialogueGroupSO>();
            createdDialogues = new Dictionary<string, DSDialogueSO>();

            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();

            addedUngroupedDialogues = new List<DSDialogueSO>();   
        }
        #region Save
        public static void Save()
        {
            CreateDefaultFolders();

            GetElementsFromGraphView();
            DSGraphSaveDataSO graphData = LoadAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
            if (graphData == null)
            {
                graphData = CreateAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
                graphData.Initialize(graphFileName);
            }


            DSDialogueContainerSO dialogueContainer = LoadAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);

            if(dialogueContainer == null)
            {
                dialogueContainer = CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);
                List<DSExposedProperty> temp = new List<DSExposedProperty>();
                temp.AddRange(graphView.exposedProperties);
                dialogueContainer.Initialize(graphFileName,temp);
            }



            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
            SaveExposedProperites(graphData);
        }
        private static void SaveExposedProperites(DSGraphSaveDataSO graphData)
        {
            graphData.ExposedProperties.AddRange(graphView.exposedProperties);
            EditorUtility.SetDirty(graphData);
        }
        private static void SaveGroups(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (DSGroup group in groups)
            {
                if (group.WasModified)
                {
                    SaveGroupToGraph(group, graphData);
                    SaveGroupToScriptableObject(group, dialogueContainer);
                    groupNames.Add(group.title);

                }
            }

            UpdateOldGroups(groupNames, graphData);
        }
        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveDataSO graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }
        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            DSDialogueGroupSO dialogueGroup = CreateAsset<DSDialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            createdDialogueGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<DSDialogueSO>());

            SaveAsset(dialogueGroup);
        }
        private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            //SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            //List<string> ungroupedNodeNames = new List<string>();

            foreach (DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);
                node.OldDialogueName = node.DialogueName;
                //if (node.Group != null)
                //{
                //    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);

                //    continue;
                //}

                //ungroupedNodeNames.Add(node.DialogueName);
                //node.WasModified = false;
            }
            addedUngroupedDialogues.ForEach(dialogue => dialogueContainer.UngroupedDialogues.Add(dialogue));
            addedUngroupedDialogues.Clear();
            UpdateDialoguesChoicesConnections();

            UpdateDeletedGroupedNodes(graphData);
            UpdateDeletedUngroupedNodes(graphData, dialogueContainer);
        }
        private static void SaveNodeToGraph(DSNode node, DSGraphSaveDataSO graphData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            DSNodeSaveData nodeData = new()
            {
                ID = node.ID,
                Name = node.DialogueName,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                DialogueType = node.DialogueType,
                Position = node.GetPosition().position,
                ExposedProperties = node.AllExposedPropertyNodeElements.ConvertAll(x => new DSExposedProperty()
                {
                    Name = x.property.Name,
                    OldName = x.property.Name,
                    Value = x.property.Value,
                }),
                Color = node.backgroundColor,
        };
            if (node.DialogueType == DSDialogueType.SingleChoice || node.DialogueType == DSDialogueType.MultipleChoice)
            {
                nodeData.CharacterSO = node.CharacterSO;
                nodeData.CharacterSprite = node.SpriteImage.image;

            }
            if (graphData.Nodes.FirstOrDefault(x => x.ID == nodeData.ID) is DSNodeSaveData repeatedData) 
            {
                graphData.Nodes.Remove(repeatedData);
            }
            graphData.Nodes.Add(nodeData);
        }
        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainerSO dialogueContainer)
        {
            DSDialogueSO dialogue;

            if (node.Group != null) //grouped
            {
                if (dialogueContainer.DialogueGroups[createdDialogueGroups[node.Group.ID]]
                    .FirstOrDefault(x => x.name == node.DialogueName) is DSDialogueSO repeatedDialogue)
                {
                    dialogueContainer.DialogueGroups[createdDialogueGroups[node.Group.ID]].Remove(repeatedDialogue);
                    RemoveAsset($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                }

                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

                dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
            }
            else // ungrouped nodes
            {
                if (node.OldDialogueName != null && node.OldDialogueName != node.DialogueName)
                {
                    if (dialogueContainer.UngroupedDialogues.FirstOrDefault(x => x.name == node.OldDialogueName) is DSDialogueSO repeatedDialogue)
                    {
                        dialogueContainer.UngroupedDialogues.Remove(repeatedDialogue);
                        RemoveAsset($"{containerFolderPath}/Global/Dialogues", node.OldDialogueName);
                    }
                }
                else
                {
                    if (dialogueContainer.UngroupedDialogues.FirstOrDefault(x => x.name == node.DialogueName) is DSDialogueSO repeatedDialogue)
                    {
                        dialogueContainer.UngroupedDialogues.Remove(repeatedDialogue);
                        RemoveAsset($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
                    }
                }
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);

                addedUngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogueName,
                node.Text,
                ConvertNodeChoicesToDialogueChoices(node.Choices),
                node.DialogueType,
                node.IsStartingNode(),
                node.AllExposedPropertyNodeElements.ConvertAll(x => x.property)
            );

            createdDialogues.Add(node.ID, dialogue);

            SaveAsset(dialogue);
        }
        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion
        #region Update
        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }
        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (DSNode node in nodes)
            {

                DSDialogueSO dialogue = createdDialogues[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }

            }
        }
        private static void UpdateDeletedGroupedNodes(DSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (graphView.deletedGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(graphView.deletedGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(graphView.deletedGroupedNodeNames);
        }
        private static void UpdateDeletedUngroupedNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            if (graphView.deletedUngroupedNodeNames != null && graphView.deletedUngroupedNodeNames.Count != 0)
            {
                foreach (string nodeNameToRemove in graphView.deletedUngroupedNodeNames)
                {

                    DSDialogueSO dialogue = LoadAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", nodeNameToRemove);
                    dialogueContainer.UngroupedDialogues.Remove(dialogue);

                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeNameToRemove);

                    if (graphData.Nodes.FirstOrDefault(x => x.Name == nodeNameToRemove) is DSNodeSaveData deletedData)
                    {
                        graphData.Nodes.Remove(deletedData);
                    }
                }
            }

            graphView.deletedUngroupedNodeNames.Clear();
        }
        #endregion
        #region Load
        public static void Load()
        {
            DSGraphSaveDataSO graphData = LoadAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            DSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
            LoadExposedProperties(graphData);
        }
        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach (DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position, true);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }
        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach (DSNodeSaveData nodeData in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);

                DSNode node = graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);

                node.OldDialogueName = nodeData.Name;
                node.ID = nodeData.ID;
                node.Choices = choices;
                node.Text = nodeData.Text;
                node.CharacterSO = nodeData.CharacterSO;
                foreach(var property in nodeData.ExposedProperties)
                {
                    ExposedPropertyNodeElement element = new ExposedPropertyNodeElement()
                    {
                        property = new DSExposedProperty()
                        {
                            Name = property.Name,
                            OldName = property.OldName,
                            Value = property.Value
                        }
                    };
                    node.AllExposedPropertyNodeElements.Add(element);
                }
              //  node.ExposedPropertyNodeElements = nodeData.ExposedProperties;
                node.Draw();

                if (nodeData.Color != Color.clear)
                {
                    node.backgroundColor = nodeData.Color;
                    node.ResetStyle();
                }
                node.LoadSpritesFromScriptableObject(nodeData.CharacterSprite);
                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);
                node.WasModified = false;

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;
                group.AddElement(node);
            }
        }
        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;

                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    DSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }
        private static void LoadExposedProperties(DSGraphSaveDataSO graphData)
        {
            graphData.ExposedProperties.ForEach((x) =>
            {
                graphView.AddPropertyToBlackboard(x);
            });
        }
        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }
        #endregion
        #region Creators
        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");

            CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }
        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }
        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }
        #endregion
        #region Removals
        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }
        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }
        #endregion
        #region Utilities
        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

            foreach (DSChoiceSaveData nodeChoice in nodeChoices)
            {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };

                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
        }
        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DSGroup);

            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DSNode node && node.WasModified)
                {
                    nodes.Add(node);

                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    DSGroup group = (DSGroup) graphElement;

                    if (group.WasModified)
                    {
                        groups.Add(group);
                    }

                    return;
                }
            });
        }
        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();

            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choiceData);
            }

            return choices;
        }
        #endregion
    }
}