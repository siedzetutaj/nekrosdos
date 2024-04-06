using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TPAllThoughtsSO))]
public class TPAllThoughtsEditor : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (TPAllThoughtsSO)target;


        if (GUILayout.Button("Create Thought", GUILayout.Height(30)))
        {
            script.CreateThought();
        }
        if (GUILayout.Button("Destroy Thought", GUILayout.Height(30)))
        {
            script.DestroyThought();
        }
    }
}

