using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{
    using Elements;

    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DSGraphView dsGraphView)
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
                    userData = DSDialogueType.SingleChoice,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    userData = DSDialogueType.MultipleChoice,
                    level = 2
                },         
                new SearchTreeEntry(new GUIContent("If (One True)", indentationIcon))
                {
                    userData = DSDialogueType.IfOneTrue,
                    level = 2
                },        
                new SearchTreeEntry(new GUIContent("If (All True)", indentationIcon))
                {
                    userData = DSDialogueType.IfAllTrue,
                    level = 2
                },                
                //new SearchTreeEntry(new GUIContent("Rectangle", indentationIcon))
                //{
                //    userData = new DSRectangle(),
                //    level = 2
                //},
                new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 2
                }
            };

            return searchTreeEntries;
        }
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case DSDialogueType.SingleChoice:
                {
                    DSSingleChoiceNode singleChoiceNode = (DSSingleChoiceNode) graphView.CreateNode("DialogueName", DSDialogueType.SingleChoice, localMousePosition);
                    graphView.AddElement(singleChoiceNode);

                    return true;
                }

                case DSDialogueType.MultipleChoice:
                {
                    DSMultipleChoiceNode multipleChoiceNode = (DSMultipleChoiceNode) graphView.CreateNode("DialogueName", DSDialogueType.MultipleChoice, localMousePosition);
                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                case DSDialogueType.IfOneTrue:
                {
                    DSIfOneTrueNode ifOneNode = (DSIfOneTrueNode)graphView.CreateNode("DialogueName", DSDialogueType.IfOneTrue, localMousePosition);
                    graphView.AddElement(ifOneNode);

                    return true;
                }        
                case DSDialogueType.IfAllTrue:
                {
                    DSIfAllTrueNode ifAllNode = (DSIfAllTrueNode)graphView.CreateNode("DialogueName", DSDialogueType.IfAllTrue, localMousePosition);
                    graphView.AddElement(ifAllNode);

                    return true;
                }

                case Group _:
                {
                    graphView.CreateGroup("DialogueGroup", localMousePosition);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}

//public class GraphElement
//{
//    public VisualElement mainContainer { get; private set; }

//    public void RefreshExpandedState()
//    {
//        VisualElement visualElement = mainContainer.Q("contents");
//        if (visualElement != null)
//        {
//            VisualElement visualElement2 = visualElement.Q("divider");
//            if (visualElement2 != null)
//            {
//                SetElementVisible(visualElement2);
//            }
//        }
//    }
//    private void SetElementVisible(VisualElement element)
//    {
//        element.style.visibility = StyleKeyword.Null;
//        element.RemoveFromClassList("hidden");
//    }
//}