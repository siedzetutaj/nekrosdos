using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace DS.Elements
{
    using Data.Save;
    using Utilities;
    using Windows;
    public class DSIfOneTrueNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.IfOneTrue;

            DSChoiceSaveData choiceTrue = new DSChoiceSaveData()
            {
                Text = "True"
            };

            Choices.Add(choiceTrue); 

            DSChoiceSaveData choiceFalse = new DSChoiceSaveData()
            {
                Text = "False"
            };

            Choices.Add(choiceFalse);
            mainContainer.RemoveFromClassList("ds-node__main-container");
            mainContainer.AddToClassList("ds-node__main-container-for-ifs");
        }
        public override void Draw()
        {
            DrawTitle();
            DrawInputPort();

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            DrawExposedPropertiesContainer("One True");

            backgroundColor = Color.green;
            mainContainer.style.backgroundColor = backgroundColor;

            RefreshExpandedState();
        }
    }  
    public class DSIfAllTrueNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.IfAllTrue;

            DSChoiceSaveData choiceTrue = new DSChoiceSaveData()
            {
                Text = "True"
            };

            Choices.Add(choiceTrue); 

            DSChoiceSaveData choiceFalse = new DSChoiceSaveData()
            {
                Text = "False"
            };

            Choices.Add(choiceFalse);
            mainContainer.RemoveFromClassList("ds-node__main-container");
            mainContainer.AddToClassList("ds-node__main-container-for-ifs");
        }
        public override void Draw()
        {
            DrawTitle();
            DrawInputPort();

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            DrawExposedPropertiesContainer("All True");

            backgroundColor = Color.green * 0.6f;
            mainContainer.style.backgroundColor = backgroundColor;

            RefreshExpandedState();
        }
    }
}
