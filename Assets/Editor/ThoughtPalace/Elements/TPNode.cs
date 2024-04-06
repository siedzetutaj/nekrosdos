using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class TPNode : Node
{
    public string ID { get; set; }
    public string DialogueName { get; set; }
    public List<TPNextThoughtSaveData> NextThoughts { get; set; }
    public TPThoughtType ThoughtType { get; set; }
    public TPThoughtSO ThoughtSO { get; set; }

    //public List<ExposedPropertyNodeElement> ExposedPropertyNodeElements { get; set; }
    protected TPGraphView graphView;
    protected Color defaultBackgroundColor;

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
        evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

        base.BuildContextualMenu(evt);
    }

    public virtual void Initialize(string nodeName, TPGraphView dsGraphView, Vector2 position)
    {
        if (ID == null)
        {
            ID = Guid.NewGuid().ToString();
        }

        DialogueName = nodeName;
        NextThoughts = new List<TPNextThoughtSaveData>();
        //ExposedPropertyNodeElements = new List<ExposedPropertyNodeElement>();

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

        /* INPUT CONTAINER */
        DrawInputPort();

        /* EXTENSION CONTAINER */
        DrawTPInformation();
        //DrawImage();
        //DrawExposedPropertiesContainer();
    }

    private void DrawTPInformation()
    {
        Box boxContainer = new Box();
        boxContainer.AddToClassList("DialogueBox");


        AddInformation(boxContainer);

        mainContainer.Add(boxContainer);
    }

    private void AddInformation(Box boxContainer)
    {
        Box Box = new Box();

        Image image = new Image();
        image.AddToClassList("Image");
        ObjectField objectField = new ObjectField()
        {
            objectType = typeof(TPThoughtSO),
            allowSceneObjects = false,
            value = ThoughtSO,
        };

        // When we change the variable from graph view.
        objectField.RegisterValueChangedCallback(value =>
        {
            ThoughtSO = value.newValue as TPThoughtSO;
            image.sprite = ThoughtSO.Sprite;
            title = ThoughtSO.Name;
        });
        if (ThoughtSO != null)
        {
            image.sprite = ThoughtSO.Sprite;
            title = ThoughtSO.Name;
        }

        Box.Add(objectField);
        Box.Add(image);

        boxContainer.Add(Box);
    }
    //protected void DrawTitle()
    //{
    //    TextField dialogueNameTextField = TPElementUtility.CreateTextField(DialogueName, null, callback =>
    //    {
    //        TextField target = (TextField)callback.target;
    //        target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

    //        DialogueName = target.value;


    //        graphView.RemoveNode(this);

    //        DialogueName = target.value;

    //        graphView.AddNode(this);

    //        return;

    //    });

    //    dialogueNameTextField.AddClasses(
    //        "ds-node__text-field",
    //        "ds-node__text-field__hidden",
    //        "ds-node__filename-text-field"
    //    );

    //    titleContainer.Insert(0, dialogueNameTextField);
        
    //}
    protected void DrawInputPort()
    {
        Port inputPort = this.CreatePort("Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

        inputContainer.Add(inputPort);
    }    

    //protected void DrawExposedPropertiesContainer(string containerTitle = null)
    //{
    //    //Load

    //    //Create New
    //    VisualElement customDataContainer = new VisualElement();
    //    customDataContainer.AddToClassList("ds-node__custom-data-container");
    //    containerTitle ??= "Dialogue Choice Effect";
    //    Foldout foldout = DSElementUtility.CreateFoldout(containerTitle);
    //    foldout.value = false;

    //    Button addPropertyButton = DSElementUtility.CreateButton("Add Property", () =>
    //    {
    //        DrawExposedPropertyList(foldout, ExposedPropertyNodeElements);
    //    });

    //    foldout.Add(addPropertyButton);
    //    if (ExposedPropertyNodeElements.Count > 0)
    //    {
    //        List<ExposedPropertyNodeElement> propertyNodeElements = new List<ExposedPropertyNodeElement>();
    //        propertyNodeElements.AddRange(ExposedPropertyNodeElements);
    //        foreach (var property in ExposedPropertyNodeElements)
    //        {
    //            DrawExposedPropertyList(foldout, propertyNodeElements, property, false);
    //        }
    //        ExposedPropertyNodeElements = propertyNodeElements;
    //    }
    //    customDataContainer.Add(foldout);

    //    extensionContainer.Add(customDataContainer);

    //}
    //protected void DrawExposedPropertyList(Foldout foldout, List<ExposedPropertyNodeElement> exposedPropertyNodeElements, ExposedPropertyNodeElement exposedPropertyElement = null, bool isNew = true)
    //{
    //    exposedPropertyElement ??= new ExposedPropertyNodeElement();
    //    exposedPropertyElement.Initialize(graphView);

    //    VisualElement propertyListContainer = new VisualElement();
    //    propertyListContainer.AddToClassList("ds-node__custom-data-container");

    //    exposedPropertyElement.listView = new ListView(graphView.exposedProperties.ConvertAll(x => x.Name))
    //    {
    //        headerTitle = "Properties",
    //        showFoldoutHeader = true,

    //    };
    //    exposedPropertyElement.listView.AddToClassList("ds-node__extension-container-height");
    //    graphView.OnExposedPropertiesListAdd += exposedPropertyElement.OnListAddProperty;
    //    graphView.OnExposedPropertiesListChange += exposedPropertyElement.OnListChangeProperty;
    //    exposedPropertyElement.listView.selectionChanged += exposedPropertyElement.OnListSelected;

    //    exposedPropertyElement.toggle = new Toggle()
    //    {
    //        text = "Wybrana opcja = null",
    //    };

    //    if (exposedPropertyElement.property != null)
    //    {
    //        exposedPropertyElement.toggle.text = $"Wybrana opcja = {exposedPropertyElement.property.Name}";
    //        exposedPropertyElement.toggle.value = exposedPropertyElement.property.Value;
    //        exposedPropertyElement.toggle.RegisterValueChangedCallback(value =>
    //        {
    //            var changingPropertyIndex = ExposedPropertyNodeElements.FindIndex(x => x == exposedPropertyElement);
    //            ExposedPropertyNodeElements[changingPropertyIndex].property.Value = value.newValue;
    //        });
    //    }
    //    propertyListContainer.Add(exposedPropertyElement.toggle);
    //    propertyListContainer.Add(exposedPropertyElement.listView);
    //    DrawDeleteButton(foldout, propertyListContainer, exposedPropertyElement);

    //    if (isNew)
    //        exposedPropertyNodeElements.Add(exposedPropertyElement);
    //    else
    //    {
    //        int index = exposedPropertyNodeElements.FindIndex(x => x.property == exposedPropertyElement.property);
    //        exposedPropertyNodeElements[index] = exposedPropertyElement;
    //    }

    //    foldout.Add(propertyListContainer);

    //}

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
    protected Image GetNewImage(string USS01 = "", string USS02 = "")
    {
        Image imagePreview = new Image();

        // Set uss class for stylesheet.
        imagePreview.AddToClassList(USS01);
        imagePreview.AddToClassList(USS02);

        return imagePreview;
    }
    #endregion
}
//[Serializable]
//public class ExposedPropertyNodeElement
//{
//    [field: SerializeField] public DSExposedProperty property;
//    [field: SerializeField] public ListView listView;
//    [field: SerializeField] public Toggle toggle;
//    [field: SerializeField] public DSGraphView graphView;

//    public virtual void Initialize(DSGraphView dsGraphView)
//    {
//        graphView = dsGraphView;
//    }
//    public void OnListSelected(IEnumerable<object> obj)
//    {
//        foreach (object objItem in obj)
//        {
//            property = graphView.exposedProperties.Find(x => x.Name == objItem.ToString());
//            toggle.text = $"Wybrana opcja = {property.Name}";
//            toggle.value = true;
//        }
//    }
//    public void OnListAddProperty(string Name)
//    {
//        listView.itemsSource.Add(Name);
//        listView.Rebuild();
//    }
//    public void OnListChangeProperty(string newName, string oldName)
//    {
//        int index = listView.itemsSource.IndexOf(oldName);
//        listView.itemsSource[index] = newName;
//        listView.Rebuild();
//    }
//}
