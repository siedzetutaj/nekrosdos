using System.Collections.Generic;
using UnityEngine;

using DS.Data;
using DS.Elements;

public class DSDialogueSO : ScriptableObject
{
    [field: SerializeField] public string DialogueName { get; set; }
    [field: SerializeField][field: TextArea()] public string Text { get; set; }
    [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
    [field: SerializeField] public DSDialogueType DialogueType { get; set; }
    [field: SerializeField] public bool IsStartingDialogue { get; set; }
    [field: SerializeField] public List<ExposedPropertyNodeElement> ExposedProperties { get; set; }


    public void Initialize(string dialogueName, string text, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue, List<ExposedPropertyNodeElement> exposedProperties)
    {
        DialogueName = dialogueName;
        Text = text;
        Choices = choices;
        DialogueType = dialogueType;
        IsStartingDialogue = isStartingDialogue;
        ExposedProperties = exposedProperties;
    }
}
