using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "TPAllThoughtSO", menuName = "ScriptableObjects/TAllPThought", order = 1)]
public class TPAllThoughtsSO : ScriptableObject
{
    [SerializeField] public List<TPThoughtSO> AllThoughts = new List<TPThoughtSO>();
#if UNITY_EDITOR
    [Header("Thought Creator")]
    public string Name;
    public Sprite Sprite;
    [field: TextArea(10, 20)] public string Description;
    public void CreateThought()
    {
        if (Name != null && Sprite != null && Description != null && !AllThoughts.Any(x => x.Name == Name)) 
        {   
            Debug.Log($"{Name} is Created");

            TPThoughtSO thought = CreateAsset<TPThoughtSO>("Assets/ThoughtPalace/Thoughts", $"{Name}Thought");
            thought.SetID();
            thought.Name = Name;
            thought.Sprite = Sprite;
            thought.Description = Description;
            EditorUtility.SetDirty(thought);
            AllThoughts.Add(thought);
            
            Name = null;
            Sprite = null;
            Description = null;
        }
        else
        {
            Debug.Log("Fill all fields under Thought Crreator/Or check name");
        }
    }
    [Header("Thought Destroyer")]
    public TPThoughtSO ThoughtTODestroy;

    public void DestroyThought()
    {
        if (ThoughtTODestroy != null)
        {
            Debug.Log($"{ThoughtTODestroy.Name} is Delated");
            AllThoughts.Remove(ThoughtTODestroy);
            AssetDatabase.DeleteAsset($"Assets/ThoughtPalace/Thoughts/{ThoughtTODestroy.Name}Thought.asset");
            ThoughtTODestroy = null;
        }
        else
        {
            Debug.Log("Pick Thought under Thought Destroyer");
        }
    }

    #region Utilites
    public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";

        T asset = LoadAsset<T>(path, assetName);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, fullPath);
        }

        return asset;
    }
    public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";

        return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }
    #endregion
#endif
}
