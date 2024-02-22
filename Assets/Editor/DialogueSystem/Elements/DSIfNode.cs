using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    public class DSIfNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.If;

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

            DrawExposedPropertiesContainer();

            defaultBackgroundColor = Color.green;
            mainContainer.style.backgroundColor = defaultBackgroundColor;

            RefreshExpandedState();
        }
    }
}
