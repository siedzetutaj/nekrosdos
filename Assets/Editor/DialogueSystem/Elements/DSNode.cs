using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Codice.CM.Client.Differences.Merge;
    using Data.Save;
    using Enumerations;
    using System.Net;
    using Utilities;
    using Windows;

    [Serializable]
    public class ExposedPropertyNodeElement 
    {
        [field: SerializeField] public DSExposedProperty exposedProperty;
        [field: SerializeField] public ListView chooseExposedProperty;
        [field: SerializeField] public TextElement exposedPropertyText;
        [field: SerializeField] public DSGraphView graphView;

        public virtual void Initialize(DSGraphView dsGraphView)
        {
            graphView = dsGraphView;
        }
        public void OnListSelected(IEnumerable<object> obj)
        {
            foreach (object objItem in obj)
            {
                exposedProperty = graphView.exposedProperties.Find(x => x.Name == objItem.ToString());
                exposedPropertyText.text = $"Wybrana opcja = {exposedProperty.Name}";
            }
        }
        public void OnListAddProperty(string Name)
        {
            chooseExposedProperty.itemsSource.Add(Name);
            chooseExposedProperty.Rebuild();
        }
        public void OnListChangeProperty(string newName, string oldName)
        {
            int index = chooseExposedProperty.itemsSource.IndexOf(oldName);
            chooseExposedProperty.itemsSource[index] = newName;
            chooseExposedProperty.Rebuild();
        }
    }
    public class DSNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }
        public DSGroup Group { get; set; }

        public List<ExposedPropertyNodeElement> ExposedPropertyNodeElements { get; set; }

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
            ExposedPropertyNodeElements = new List<ExposedPropertyNodeElement>();
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

            DrawExposedPropertiesContainer();
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
        protected void DrawExposedPropertiesContainer()
        {
            //Load

            //Create New
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            Foldout foldout = DSElementUtility.CreateFoldout("Dialogue Choice Effect");
            foldout.value = false;

            Button addPropertyButton = DSElementUtility.CreateButton("Add Property", () =>
            {
                DrawExposedPropertyList(foldout,ExposedPropertyNodeElements);
            });

            foldout.Add(addPropertyButton);
            if (ExposedPropertyNodeElements.Count > 0)
            {
                List<ExposedPropertyNodeElement> propertyNodeElements = new List<ExposedPropertyNodeElement>();
                propertyNodeElements.AddRange(ExposedPropertyNodeElements);
                foreach (var property in ExposedPropertyNodeElements)
                {
                    DrawExposedPropertyList(foldout, propertyNodeElements, property, false);
                }
                ExposedPropertyNodeElements = propertyNodeElements;
            }
            customDataContainer.Add(foldout);

            extensionContainer.Add(customDataContainer);

        }
        protected void DrawExposedPropertyList(Foldout foldout, List<ExposedPropertyNodeElement> exposedPropertyNodeElements, ExposedPropertyNodeElement exposedPropertyElement = null,bool isNew = true)
        {
            exposedPropertyElement ??= new ExposedPropertyNodeElement();
            exposedPropertyElement.Initialize(graphView);

            VisualElement propertyListContainer = new VisualElement();
            propertyListContainer.AddToClassList("ds-node__custom-data-container");

            exposedPropertyElement.chooseExposedProperty = new ListView(graphView.exposedProperties.ConvertAll(x => x.Name))
            {
                headerTitle = "Properties",
                showFoldoutHeader = true,

            };
            exposedPropertyElement.chooseExposedProperty.AddToClassList("ds-node__extension-container-height");
            graphView.OnExposedPropertiesListAdd += exposedPropertyElement.OnListAddProperty;
            graphView.OnExposedPropertiesListChange += exposedPropertyElement.OnListChangeProperty;
            exposedPropertyElement.chooseExposedProperty.selectionChanged += exposedPropertyElement.OnListSelected;

            exposedPropertyElement.exposedPropertyText = new TextElement()
            {
                text = "Wybrana opcja = null"
            };

            if (exposedPropertyElement.exposedProperty != null)
            {
                exposedPropertyElement.exposedPropertyText.text = $"Wybrana opcja = {exposedPropertyElement.exposedProperty.Name}";
            }
            propertyListContainer.Add(exposedPropertyElement.exposedPropertyText);
            propertyListContainer.Add(exposedPropertyElement.chooseExposedProperty);
            DrawDeleteButton(foldout, propertyListContainer,exposedPropertyElement);

            if(isNew)
                exposedPropertyNodeElements.Add(exposedPropertyElement);
            else
            {
                int index = exposedPropertyNodeElements.FindIndex(x => x.exposedProperty == exposedPropertyElement.exposedProperty);
                exposedPropertyNodeElements[index] = exposedPropertyElement;
            }
                
            foldout.Add(propertyListContainer);

        }
        private void DrawDeleteButton(Foldout foldout, VisualElement container, ExposedPropertyNodeElement element)
        {
            Button deleteButton = DSElementUtility.CreateButton("X", () =>
            {
                //Choices.Remove(choiceData);

                element.exposedProperty = null;
                element.exposedPropertyText = null;
                foldout.Remove(container);
            });

            deleteButton.AddToClassList("ds-node__button");
            container.Insert(0,deleteButton);
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



        #endregion
    }
}