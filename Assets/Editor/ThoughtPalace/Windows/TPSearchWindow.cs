using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class TPSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private TPGraphView graphView;
    private Texture2D indentationIcon;

    public void Initialize(TPGraphView dsGraphView)
    {
        graphView = dsGraphView;

        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0, 0, Color.clear);
        indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    userData = TPThoughtType.Information,
                    level = 2
                },
                //new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                //{
                //    userData = TPThoughtType.MultipleChoice,
                //    level = 2
                //},
                //new SearchTreeEntry(new GUIContent("If (One True)", indentationIcon))
                //{
                //    userData = TPThoughtType.IfOneTrue,
                //    level = 2
                //},
                //new SearchTreeEntry(new GUIContent("If (All True)", indentationIcon))
                //{
                //    userData = TPThoughtType.IfAllTrue,
                //    level = 2
                //},
                //new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
                //new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                //{
                //    userData = new Group(),
                //    level = 2
                //}
            };

        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

        switch (SearchTreeEntry.userData)
        {
            case TPThoughtType.Information:
                {
                    TPInformationNode singleChoiceNode = (TPInformationNode)graphView.CreateNode("DialogueName", TPThoughtType.Information, localMousePosition);
                    graphView.AddElement(singleChoiceNode);

                    return true;
                }

            default:
                {
                    return false;
                }
        }
    }
}
