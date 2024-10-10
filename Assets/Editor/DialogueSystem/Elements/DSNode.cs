using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using TMPro;
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
        public List<ExposedPropertyNodeElement> AllExposedPropertyNodeElements { get; set; }
        public DSCharacterSO CharacterSO { get; set; }
        public Image SpriteImage { get; set; }
        
        public bool WasModified {  get; set; }

        private VisualElement LeftContainer { get; set; }
        private DropdownField SpriteDropdown { get; set; }

        protected DSGraphView graphView;
        public Color backgroundColor;

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
            AllExposedPropertyNodeElements = new List<ExposedPropertyNodeElement>();
            Text = "Dialogue text.";

            SetPosition(new Rect(position, Vector2.zero));

            graphView = dsGraphView;
            backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

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
            DrawLeftSide();
            DrawExposedPropertiesContainer();
            DrawRightSide();
            // DrawDialogueTextField();

            RefreshExpandedState();
        }

        protected void DrawLeftSide()
        {
            LeftContainer = new VisualElement();
            LeftContainer.AddToClassList("ds-node__left-container");

            ObjectField ScriptableObjectField = new ObjectField()
            {
                objectType = typeof(DSCharacterSO),
                value = CharacterSO,
            };
            ScriptableObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is DSCharacterSO character)
                {
                    // Load sprites from selected object
                    CharacterSO = character;
                    LoadSpritesFromScriptableObject();
                }
            });
            LeftContainer.Add(ScriptableObjectField);
            // Sprite display area
            VisualElement SpriteContainer = new VisualElement();
            SpriteContainer.AddToClassList("ds-node__sprite-container");
            SpriteImage = new Image();
            SpriteContainer.Add(SpriteImage);
            LeftContainer.Add(SpriteContainer);

            // Dropdown for selecting the sprite
            SpriteDropdown = new DropdownField();
            SpriteDropdown.RegisterValueChangedCallback(evt =>
            {
                UpdateSpriteDisplay(evt.newValue);
            });
            LeftContainer.Add(SpriteDropdown);
            extensionContainer.Add(LeftContainer);
        }
        protected void DrawRightSide()
        {
            VisualElement rightContainer = new VisualElement();
            rightContainer.AddToClassList("ds-node__right-container");
            //textField = new TextField("Text");
            //rightContainer.Add(textField);

            // Add left and right containers to main container
            extensionContainer.Add(DrawDialogueTextField());
            //    extensionContainer.Add(mainContainer);

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
        protected void DrawExposedPropertiesContainer(string containerTitle = null)
        {
            //Load

            //Create New
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__foldout-data-container");
            containerTitle ??= "Dialogue Choice Effect";
            Foldout foldout = DSElementUtility.CreateFoldout(containerTitle);
            foldout.value = false;

            foldout.RegisterValueChangedCallback(evt =>
            {
                customDataContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            Button addPropertyButton = DSElementUtility.CreateButton("Add Property", () =>
            {
                DrawExposedPropertyList(customDataContainer, AllExposedPropertyNodeElements);
            });

            customDataContainer.Add(addPropertyButton);
            if (AllExposedPropertyNodeElements.Count > 0)
            {
                List<ExposedPropertyNodeElement> propertyNodeElements = new List<ExposedPropertyNodeElement>();
                propertyNodeElements.AddRange(AllExposedPropertyNodeElements);
                foreach (var property in AllExposedPropertyNodeElements)
                {
                    DrawExposedPropertyList(customDataContainer, propertyNodeElements, property, false);
                }
                AllExposedPropertyNodeElements = propertyNodeElements;
            }

            extensionContainer.Add(customDataContainer);
            customDataContainer.style.display = DisplayStyle.None;
            if (containerTitle == "Dialogue Choice Effect")
                LeftContainer.Add(foldout);
            else
                extensionContainer.Add(foldout);
        }
        protected void DrawExposedPropertyList(VisualElement visualElement, List<ExposedPropertyNodeElement> exposedPropertyNodeElements, ExposedPropertyNodeElement exposedPropertyElement = null, bool isNew = true)
        {
            exposedPropertyElement ??= new ExposedPropertyNodeElement();
            exposedPropertyElement.Initialize(graphView);

            VisualElement propertyListContainer = new VisualElement();
            propertyListContainer.AddToClassList("ds-node__foldout-data-container");

            exposedPropertyElement.listView = new ListView(graphView.exposedProperties.ConvertAll(x => x.Name))
            {
                headerTitle = "Properties",
                showFoldoutHeader = true,

            };
            exposedPropertyElement.listView.AddToClassList("ds-node__extension-container-height");
            graphView.OnExposedPropertiesListAdd += exposedPropertyElement.OnListAddProperty;
            graphView.OnExposedPropertiesListChange += exposedPropertyElement.OnListChangeProperty;
            exposedPropertyElement.listView.selectionChanged += exposedPropertyElement.OnListSelected;

            exposedPropertyElement.toggle = new Toggle()
            {
                text = "Wybrana opcja = null",
            };

            if (exposedPropertyElement.property != null)
            {
                exposedPropertyElement.toggle.text = $"Wybrana opcja = {exposedPropertyElement.property.Name}";
                exposedPropertyElement.toggle.value = exposedPropertyElement.property.Value;
                exposedPropertyElement.toggle.RegisterValueChangedCallback(value =>
                {
                    var changingPropertyIndex = AllExposedPropertyNodeElements.FindIndex(x => x == exposedPropertyElement);
                    AllExposedPropertyNodeElements[changingPropertyIndex].property.Value = value.newValue;
                });
            }
            propertyListContainer.Add(exposedPropertyElement.toggle);
            propertyListContainer.Add(exposedPropertyElement.listView);
            DrawDeleteButton(visualElement, propertyListContainer, exposedPropertyElement);

            if (isNew)
                exposedPropertyNodeElements.Add(exposedPropertyElement);
            else
            {
                int index = exposedPropertyNodeElements.FindIndex(x => x.property == exposedPropertyElement.property);
                exposedPropertyNodeElements[index] = exposedPropertyElement;
            }

            visualElement.Add(propertyListContainer);

        }
        private void DrawDeleteButton(VisualElement visualElement, VisualElement container, ExposedPropertyNodeElement element)
        {
            Button deleteButton = DSElementUtility.CreateButton("X", () =>
            {
                AllExposedPropertyNodeElements.Remove(element);
                visualElement.Remove(container);
            });

            deleteButton.AddToClassList("ds-node__button");
            container.Insert(0, deleteButton);
        }
        protected TextField DrawDialogueTextField()
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
            return textTextField;
            //extensionContainer.Add(customDataContainer);
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
            Port inputPort = (Port)inputContainer.Children().First();

            return !inputPort.connected;
        }
        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }
        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = backgroundColor;
            mainContainer.style.backgroundColor = backgroundColor;
        }
        public void LoadSpritesFromScriptableObject(Texture dataToLoad = null)
        {
            // Example: Assuming your ScriptableObject has a List<Sprite> or similar field
            if (CharacterSO == null)
                return;
            List<string> spriteNames = CharacterSO.Emotions.Select(sprite => sprite.name).ToList(); // Populate with sprite names from the object
            SpriteDropdown.choices = spriteNames;

            if (dataToLoad != null)
            {
                var spriteName = spriteNames.FirstOrDefault(x => x == dataToLoad.name);
                SpriteDropdown.value = spriteName;
                UpdateSpriteDisplay(spriteName);
                return;
            }
            // Set the first sprite to be displayed (if available)

            if (spriteNames.Count > 0)
            {
                SpriteDropdown.value = spriteNames[0];
                UpdateSpriteDisplay(spriteNames[0]);
            }
        }
        // Update the sprite image based on dropdown selection
        private void UpdateSpriteDisplay(string spriteName)
        {
            Sprite selectedSprite = GetSpriteByName(spriteName);

            if (selectedSprite != null)
            {
                // Set the sprite to the image
                SpriteImage.image = selectedSprite.texture;

                // Set the size to match container
                SpriteImage.style.width = 150;
                SpriteImage.style.height = 170;
                SpriteImage.style.marginLeft = -30;
                // Adjust the image position to center if needed
                SpriteImage.scaleMode = ScaleMode.ScaleToFit;
            }
        }

        // Mock method to get sprite by name, replace with real implementation
        private Sprite GetSpriteByName(string spriteName)
        {
            Sprite sprite = CharacterSO.Emotions.FirstOrDefault(x => x.name == spriteName);
            if (sprite != null)
            {
                return sprite;
            }
            return null; // Replace with real implementation
        }
        #endregion
    }
    [Serializable]
    public class ExposedPropertyNodeElement
    {
        [field: SerializeField] public DSExposedProperty property;
        [field: SerializeField] public ListView listView;
        [field: SerializeField] public Toggle toggle;
        [field: SerializeField] public DSGraphView graphView;

        public virtual void Initialize(DSGraphView dsGraphView)
        {
            graphView = dsGraphView;
        }
        public void OnListSelected(IEnumerable<object> obj)
        {
            foreach (object objItem in obj)
            {
                property = graphView.exposedProperties.Find(x => x.Name == objItem.ToString());
                toggle.text = $"Wybrana opcja = {property.Name}";
                toggle.value = true;
                toggle.RegisterValueChangedCallback(value =>
                {
                    property.Value = value.newValue;
                });
            }
        }
        public void OnListAddProperty(string Name)
        {
            listView.itemsSource.Add(Name);
            listView.Rebuild();
        }
        public void OnListChangeProperty(string newName, string oldName)
        {
            int index = listView.itemsSource.IndexOf(oldName);
            listView.itemsSource[index] = newName;
            listView.Rebuild();
        }
    }

}