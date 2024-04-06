using System;
using UnityEngine;


[Serializable]
public class DSDialogueChoiceData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public DSDialogueSO NextDialogue { get; set; }

    [field: SerializeField] public bool WasDisplayed = false;
}
