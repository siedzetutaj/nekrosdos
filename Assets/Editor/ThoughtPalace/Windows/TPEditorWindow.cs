using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

    public class TPEditorWindow : EditorWindow
    {
        private TPGraphView graphView;

        private readonly string defaultFileName = "ThoughtFileName";

        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;
        private Button blackboardButton;

        [MenuItem("Window/TP/Thought Graph")]
        public static void Open()
        {
            GetWindow<TPEditorWindow>("Thought Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
            EditorApplication.quitting += Save;
    }

    private void AddGraphView()
        {
            graphView = new TPGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = TPElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = TPElementUtility.CreateButton("Save", () => Save());

            Button loadButton = TPElementUtility.CreateButton("Load", () => Load());
            Button clearButton = TPElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = TPElementUtility.CreateButton("Reset", () => ResetGraph());

            miniMapButton = TPElementUtility.CreateButton("Minimap", () => ToggleMiniMap());
            //blackboardButton = TPElementUtility.CreateButton("Variables", ()=>ToggleBlackboard());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);
            toolbar.Add(blackboardButton);

            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }
    //Save trzeba przerobiæ 
        #region Utilities
        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

                return;
            }

        TPIOUtility.Initialize(graphView, fileNameTextField.value);
        TPIOUtility.Save();
    }
    private void Load()
    {
        string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/ThoughtPalace/Graphs", "asset");

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        Clear();

        TPIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
        TPIOUtility.Load();
    }
    private void Clear()
        {
            graphView.ClearGraph();
        }
        private void ResetGraph()
        {
            Clear();

            UpdateFileName(defaultFileName);
        }
        #endregion
        #region Togglers
        private void ToggleMiniMap()
        {
            graphView.ToggleMiniMap();

            miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }   
        //private void ToggleBlackboard()
        //{
        //    graphView.ToggleBlackboard();

        //    blackboardButton.ToggleInClassList("ds-toolbar__button__selected");
        //}
        #endregion
        #region Functions 
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion

    }
