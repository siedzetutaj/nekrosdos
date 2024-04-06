using UnityEngine;



public class DSDialogue : MonoBehaviour
{
    /* Dialogue Scriptable Objects */
    [SerializeField] public DSDialogueContainerSO dialogueContainer;
    [SerializeField] private DSDialogueGroupSO dialogueGroup;
    [SerializeField] public DSDialogueSO dialogue;

    /* Filters */
    [SerializeField] private bool groupedDialogues;
    [SerializeField] private bool startingDialoguesOnly;

    /* Indexes */
    [SerializeField] private int selectedDialogueGroupIndex;
    [SerializeField] private int selectedDialogueIndex;
}
