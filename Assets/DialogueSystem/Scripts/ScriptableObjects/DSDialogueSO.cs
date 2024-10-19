using System.Collections.Generic;
using UnityEngine;


public class DSDialogueSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; set; }
    [field: SerializeField] public string DialogueName { get; set; }
    [field: SerializeField][field: TextArea()] public string Text { get; set; }
    [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
    [field: SerializeField] public DSDialogueType DialogueType { get; set; }
    [field: SerializeField] public bool IsStartingDialogue { get; set; }
    [field: SerializeField] public List<DSExposedProperty> ExposedProperties { get; set; }


    public void Initialize(string dialogueID, string dialogueName, string text, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue, List<DSExposedProperty> exposedProperties)
    {
        ID = dialogueID;
        DialogueName = dialogueName;
        Text = text;
        Choices = choices;
        DialogueType = dialogueType;
        IsStartingDialogue = isStartingDialogue;
        ExposedProperties = exposedProperties;
    }
}
