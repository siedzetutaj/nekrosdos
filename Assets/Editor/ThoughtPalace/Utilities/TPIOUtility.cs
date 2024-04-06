using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public static class TPIOUtility
{
    private static TPGraphView graphView;

    private static string graphFileName;
    private static string containerFolderPath;

    private static List<TPNode> nodes;

    private static Dictionary<string, TPThoughtSO> createdThoughts;

    private static Dictionary<string, TPNode> loadedNodes;

    public static void Initialize(TPGraphView dsGraphView, string graphName)
    {
        graphView = dsGraphView;

        graphFileName = graphName;
        containerFolderPath = $"Assets/ThoughtPalace/ThoughtsConnections";

        nodes = new List<TPNode>();

        createdThoughts = new Dictionary<string, TPThoughtSO>();

        loadedNodes = new Dictionary<string, TPNode>();
    }
    #region Save
    public static void Save()
    {
        CreateDefaultFolders();

        GetElementsFromGraphView();

        TPGraphSaveDataSO graphData = CreateAsset<TPGraphSaveDataSO>("Assets/Editor/ThoughtPalace/Graphs", $"{graphFileName}Graph");

        graphData.Initialize(graphFileName);

        //DSDialogueContainerSO dialogueContainer = CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);
        //dialogueContainer.Initialize(graphFileName, temp);

        //SaveGroups(graphData, dialogueContainer);
        SaveNodeToGraph(graphData);
        SaveThoughtConnectionsToScriptableObject();

        SaveAsset(graphData);
        //SaveAsset(dialogueContainer);
    }


    //private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
    //{
    //    SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
    //    List<string> ungroupedNodeNames = new List<string>();

    //    foreach (DSNode node in nodes)
    //    {
    //        SaveNodeToGraph(node, graphData);
    //        SaveNodeToScriptableObject(node, dialogueContainer);

    //        if (node.Group != null)
    //        {
    //            groupedNodeNames.AddItem(node.Group.title, node.DialogueName);

    //            continue;
    //        }

    //        ungroupedNodeNames.Add(node.DialogueName);
    //    }

    //    UpdateDialoguesChoicesConnections();

    //    UpdateOldGroupedNodes(groupedNodeNames, graphData);
    //    UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
    //}
    private static void SaveNodeToGraph(TPGraphSaveDataSO graphData)
    {
        foreach (TPNode node in nodes)
        {

            List<TPNextThoughtSaveData> next = CloneNodeChoices(node.NextThoughts);

            TPNodeSaveData nodeData = new TPNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogueName,
                NextThoughts = next,
                ThoughtType = node.ThoughtType,
                ThoughtSO = node.ThoughtSO,
                Position = node.GetPosition().position,
            };

            graphData.Nodes.Add(nodeData);
        }
    }
    private static void SaveThoughtConnectionsToScriptableObject()
    {
        TPThoughtConections thoughtConections = CreateAsset<TPThoughtConections>(containerFolderPath, graphFileName);
        thoughtConections.Reset();
        foreach (TPNode node in nodes)
        {
            
            List<TPNextThoughtSaveData> nexts = CloneNodeChoices(node.NextThoughts);

            foreach (TPNextThoughtSaveData next in nexts)
            {
                string connection = node.ID + next.NodeID;
                thoughtConections.AddConnection(connection);
            }
        }
        SaveAsset(thoughtConections);
    }
    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endregion
    #region Update

    //private static void UpdateDialoguesChoicesConnections()
    //{
    //    foreach (DSNode node in nodes)
    //    {
    //        DSDialogueSO dialogue = createdThoughts[node.ID];

    //        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
    //        {
    //            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

    //            if (string.IsNullOrEmpty(nodeChoice.NodeID))
    //            {
    //                continue;
    //            }

    //            dialogue.Choices[choiceIndex].NextDialogue = createdThoughts[nodeChoice.NodeID];

    //            SaveAsset(dialogue);
    //        }
    //    }
    //}

    //private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveDataSO graphData)
    //{
    //    if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
    //    {
    //        List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

    //        foreach (string nodeToRemove in nodesToRemove)
    //        {
    //            RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
    //        }
    //    }

    //    graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
    //}
    #endregion
    #region Load
    public static void Load()
    {
        TPGraphSaveDataSO graphData = LoadAsset<TPGraphSaveDataSO>("Assets/Editor/ThoughtPalace/Graphs", graphFileName);

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

        TPEditorWindow.UpdateFileName(graphData.FileName);

        LoadNodes(graphData.Nodes);
        LoadNodesConnections();
    }

    private static void LoadNodes(List<TPNodeSaveData> nodes)
    {
        foreach (TPNodeSaveData nodeData in nodes)
        {
            List<TPNextThoughtSaveData> next = CloneNodeChoices(nodeData.NextThoughts);

            TPNode node = graphView.CreateNode(nodeData.Name, nodeData.ThoughtType, nodeData.Position, false);

            node.ID = nodeData.ID;
            node.NextThoughts = next;
            node.ThoughtSO = nodeData.ThoughtSO;
            node.Draw();

            graphView.AddElement(node);

            loadedNodes.Add(node.ID, node);
        }
    }
    private static void LoadNodesConnections()
    {
        foreach (KeyValuePair<string, TPNode> loadedNode in loadedNodes)
        {
            foreach (Port outputPort in loadedNode.Value.outputContainer.Children())
            {
                List<TPNextThoughtSaveData> saveDataList = (List<TPNextThoughtSaveData>)outputPort.userData;
                foreach (TPNextThoughtSaveData nextThoughtSaveData in saveDataList)
                {

                    if (string.IsNullOrEmpty(nextThoughtSaveData.NodeID))
                    {
                        continue;
                    }

                    TPNode nextNode = loadedNodes[nextThoughtSaveData.NodeID];

                    Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

                    Edge edge = outputPort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }
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
        CreateFolder("Assets/Editor/ThoughtPalace", "Graphs");

        CreateFolder("Assets", "ThoughtPalace");
        CreateFolder("Assets/ThoughtPalace", "ThoughtsConnections");
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
    //private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
    //{
    //    List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

    //    foreach (DSChoiceSaveData nodeChoice in nodeChoices)
    //    {
    //        DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
    //        {
    //            Text = nodeChoice.Text
    //        };

    //        dialogueChoices.Add(choiceData);
    //    }

    //    return dialogueChoices;
    //}
    private static void GetElementsFromGraphView()
    {

        graphView.graphElements.ForEach(graphElement =>
        {
            if (graphElement is TPNode node)
            {
                nodes.Add(node);

                return;
            }
        });
    }
    private static List<TPNextThoughtSaveData> CloneNodeChoices(List<TPNextThoughtSaveData> nodeChoices)
    {
        List<TPNextThoughtSaveData> choices = new List<TPNextThoughtSaveData>();

        foreach (TPNextThoughtSaveData choice in nodeChoices)
        {
            TPNextThoughtSaveData choiceData = new TPNextThoughtSaveData()
            {
                NodeID = choice.NodeID
            };

            choices.Add(choiceData);
        }

        return choices;
    }
    #endregion
}
[Serializable]
public class TPNextThoughtSaveData
{
    [field: SerializeField] public string NodeID { get; set; }
}

[Serializable]
public class TPNodeSaveData
{
    [field: SerializeField] public string ID { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public TPThoughtSO ThoughtSO { get; set; }
    [field: SerializeField] public List<TPNextThoughtSaveData> NextThoughts { get; set; }
    [field: SerializeField] public TPThoughtType ThoughtType { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }
}


