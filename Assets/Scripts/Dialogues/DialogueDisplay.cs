using DS;
using DS.Data;
using DS.ScriptableObjects;
using DS.Enumerations;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private DSDialogue startingDialogue;
    [SerializeField] private GameObject contentUI;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private GameObject buttonPrefab;

    private bool isSingleChoice;
    private DSDialogueSO currentDialogue;
    private List<GameObject> buttonList = new List<GameObject>();

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
            isSingleChoice = false;
            CreateButtons();
        }
    }
    private void CreateButtons()
    {
        foreach (DSDialogueChoiceData dialogueSO in currentDialogue.Choices)
        {
            GameObject button = Instantiate(buttonPrefab, contentUI.transform);
            DialogueButton dButton = button.GetComponent<DialogueButton>();
            dButton.text.text = dialogueSO.Text;
            dButton.choiceNumber = currentDialogue.Choices.IndexOf(dialogueSO);
            dButton.button.onClick.AddListener(() =>
            {
                AddButtonTextToContainer(dialogueSO.Text);
                OnOptionChosen(dButton.choiceNumber);
            });
            buttonList.Add(button);
        }
    }
    private void AddButtonTextToContainer(string text)
    {
        textUI.text += text;
        textUI.text += "\n";
        textUI.text += "\n";
    }
    private void OnOptionChosen(int choiceIndex = 0)
    {
        isSingleChoice = false;
        RemoveButonsFromContainer();

        DSDialogueSO nextDialogue = currentDialogue.Choices[choiceIndex].NextDialogue;

        if (nextDialogue.DialogueType == DSDialogueType.If)
        {
            bool isTrue = false;
            foreach (var property in nextDialogue.ExposedProperties)
            {
                if (property.property.Value)
                    isTrue = true;
                break;
            }
            if (isTrue)
            {
                nextDialogue = nextDialogue.Choices[0].NextDialogue;
            }
            else
            {
                nextDialogue = nextDialogue.Choices[1].NextDialogue;
            }

        }

        if (nextDialogue == null)
        {
            return; // No more dialogues to show, do whatever you want, like setting the currentDialogue to the startingDialogue
        }

        currentDialogue = nextDialogue;

        ShowText();
    }

    private void RemoveButonsFromContainer()
    {
        if (buttonList.Count >= 1)
        {
            foreach (GameObject button in buttonList)
            {
                Destroy(button);
            }
            buttonList.Clear();
        }
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
