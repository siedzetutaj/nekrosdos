using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceneEnumGenerator : EditorWindow
{
    private const string enumName = "SceneNames";
    private const string filePath = "Assets/Scripts/Enums/SceneNames.cs"; // Œcie¿ka do pliku enum

    [MenuItem("Tools/Generate Scene Enum")]
    public static void GenerateSceneEnum()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        string[] sceneNames = sceneGuids.Select(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid))).ToArray();

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");

            foreach (string sceneName in sceneNames)
            {
                writer.WriteLine("    " + sceneName + ",");
            }

            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        Debug.Log(enumName + " enum has been updated with the current scene names.");
    }
}
