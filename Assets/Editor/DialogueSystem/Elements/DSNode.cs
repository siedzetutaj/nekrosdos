using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }
        public DSGroup Group { get; set; }

        public DSExposedProperty exposedProperty;
        protected ListView chooseExposedProperty;
        protected TextElement exposedPropertyText;


        protected DSGraphView graphView;
        private Color defaultBackgroundColor;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();

            DialogueName = nodeName;
            Choices = new List<DSChoiceSaveData>();
            Text = "Dialogue text.";

            SetPosition(new Rect(position, Vector2.zero));

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }
        #region Draw
        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            DrawTitle();

            /* INPUT CONTAINER */

            DrawInputPort();

            /* EXTENSION CONTAINER */

            DrawExposedContainer();
            DrawDialogueTextField();
        }

        protected void DrawTitle()
        {
            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                DSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                DialogueName = target.value;

                graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);
        }
        protected void DrawInputPort()
        {
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);
        }
        protected void DrawExposedContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            Foldout foldout = DSElementUtility.CreateFoldout("Dialogue Choice Effect");
            foldout.value = false;
            DrawExposedPropertyList();
            DrawExposedPropertyText();

            foldout.Add(exposedPropertyText);
            foldout.Add(chooseExposedProperty);
            customDataContainer.Add(foldout);

            extensionContainer.Add(customDataContainer);
        }
        protected void DrawExposedPropertyList()
        {
            chooseExposedProperty = new ListView(graphView.exposedProperties.ConvertAll(x => x.Name))
            {
                headerTitle = "Properties",
                showFoldoutHeader = true,

            };
            chooseExposedProperty.AddToClassList("ds-node__extension-container-height");
            graphView.OnExposedPropertiesListAdd += OnListAddProperty;
            graphView.OnExposedPropertiesListChange += OnListChangeProperty;
            chooseExposedProperty.selectionChanged += OnListSelected;
        }
        protected void DrawExposedPropertyText()
        {
            exposedPropertyText = new TextElement()
            {
                text = "Wybrana opcja = null"
            };
            if (exposedProperty != null)
            {
                exposedPropertyText.text = $"Wybrana opcja = {exposedProperty.Name}";
            }
        }
        protected void DrawDialogueTextField()
        {
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

            TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback => Text = callback.newValue);

            textTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__quote-text-field"
            );

            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
        }
        #endregion
        #region Disconnections
        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }
        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }
        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }
        #endregion
        #region Utilities
        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }
        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }
        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        private void OnListSelected(IEnumerable<object> obj)
        {
            foreach (object objItem in obj)
            {
                exposedProperty = graphView.exposedProperties.Find(x => x.Name == objItem.ToString());
                exposedPropertyText.text = $"Wybrana opcja = {exposedProperty.Name}";
            }
        }
        private void OnListAddProperty(string Name)
        {
            chooseExposedProperty.itemsSource.Add(Name);
            chooseExposedProperty.Rebuild();
        }
        private void OnListChangeProperty(string newName, string oldName)
        {
            int index = chooseExposedProperty.itemsSource.IndexOf(oldName);
            chooseExposedProperty.itemsSource[index] = newName;
            chooseExposedProperty.Rebuild();
        }
        #endregion
    }
}