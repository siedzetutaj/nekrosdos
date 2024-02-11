using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using System;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using Utilities;
    using Windows;
    public class DSIfNode : DSNode
    {
        public DSExposedProperty checkProperty;
        public ListView choosePropertyListView;
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
            DrawPropertyList();
            
            RefreshExpandedState();
        }


        private void DrawPropertyList()
        {
            extensionContainer.AddClasses("ds-node__extension-container-height");
            choosePropertyListView = new ListView(graphView.exposedProperties.ConvertAll(x=>x.Name)) 
            {
                headerTitle = "Properties",
                showFoldoutHeader = true,
                
            };
            graphView.OnExposedPropertiesListAdd += OnAddProperty;
            graphView.OnExposedPropertiesListChange += OnChangeProperty;
            choosePropertyListView.selectionChanged += OnSelected;
            extensionContainer.Add(choosePropertyListView);
        }

        private void OnSelected(IEnumerable<object> obj)
        {
            foreach (object objItem in obj)
            {
                checkProperty = graphView.exposedProperties.Find(x => x.Name == objItem.ToString());
                Debug.Log(checkProperty);
                Debug.Log(checkProperty.Name);
                Debug.Log(checkProperty.Value);
            }
        }

        public void OnAddProperty(string Name)
        {
            choosePropertyListView.itemsSource.Add(Name);
            choosePropertyListView.Rebuild();
        }
        public void OnChangeProperty(string newName, string oldName)
        {
            int index = choosePropertyListView.itemsSource.IndexOf(oldName);
            choosePropertyListView.itemsSource[index] = newName;
            choosePropertyListView.Rebuild();
        }
    }
}
