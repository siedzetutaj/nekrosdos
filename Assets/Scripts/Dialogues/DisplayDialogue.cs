using DS;
using DS.ScriptableObjects;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;

public class DisplayDialogue : MonoBehaviour
{
    [SerializeField] private DSDialogue startingDialogue;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private bool isSingleChoice;

    private DSDialogueSO currentDialogue;

    private void Awake()
    {
        textUI.text = "";
        currentDialogue = startingDialogue.dialogue;
        ShowText();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSingleChoice)
        {
            OnOptionChosen();
        }
    }
    private void ShowText()
    {
        textUI.text += currentDialogue.Text;
        textUI.text += "\n";
        textUI.text += "\n";

        if (currentDialogue.Choices.Count == 1)
        {
            isSingleChoice = true;
        }
        else if (currentDialogue.Choices.Count > 1)
        {
            // buttons 
        }        
    }

    private void OnOptionChosen(int choiceIndex = 0)
    {
        isSingleChoice = false;

        DSDialogueSO nextDialogue = currentDialogue.Choices[choiceIndex].NextDialogue;

        if (nextDialogue == null)
        {
            return; // No more dialogues to show, do whatever you want, like setting the currentDialogue to the startingDialogue
        }

        currentDialogue = nextDialogue;

        ShowText();
    }

    public static Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = text
        };

        return button;
    }
}
