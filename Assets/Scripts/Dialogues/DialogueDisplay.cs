using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class DialogueDisplay : MonoBehaviourSingleton<DialogueDisplay>
{
    [Header("Dialogue")]
    public DSDialogue startingDialogue;
    
    [Header("UI")]
    [SerializeField] private GameObject _scrollViewUI;
    [SerializeField] private GameObject _containerUI;
    [SerializeField] private GameObject _contentUI;
    [SerializeField] private TextMeshProUGUI _textUI;
    [SerializeField] private GameObject _buttonPrefab;
    
    [Header("Components")]
    [SerializeField] private InputSystem _inputSystem;

    [Header("Debug")]
    [SerializeField] private List<DSExposedProperty> _allExposedProperties;

    private bool _isSingleChoice;
    private DSDialogueSO _currentDialogue;
    private List<GameObject> _buttonList = new List<GameObject>();
    private ScrollRect _scrollRect;

    public void Initialize()
    {
        UIInformationHolder uiInformationHolder = UIInformationHolder.Instance;

        _scrollViewUI = uiInformationHolder.DialogueScrollViewUI;
        _containerUI = uiInformationHolder.DialogueContinerUI;
        _contentUI = uiInformationHolder.DialogueContentUI;
        _textUI = uiInformationHolder.DialogueTextUI;

        _containerUI.SetActive(false);
        _scrollRect = _scrollViewUI.GetComponent<ScrollRect>();
        _textUI.text = "";

        _inputSystem = InputSystem.Instance;
        _inputSystem.onDialogueLeftClickDown += OnLeftClik;
    }
    private void OnDisable()
    {
        _inputSystem.onDialogueLeftClickDown -= OnLeftClik;
    }
    public void StartDisplaying()
    {
        _inputSystem.mouseInput.DialogueInputs.Enable();
        _containerUI.SetActive(true);
        _currentDialogue = startingDialogue.dialogue;
        _allExposedProperties = startingDialogue.dialogueContainer.ExposedProperties;
        ExposedProperties();
        ShowText();
    }
    private void ShowText()
    {
        _textUI.text += _currentDialogue.Text;
        _textUI.text += "\n";
        _textUI.text += "\n";

        if (_currentDialogue.Choices.Count == 1)
        {
            _isSingleChoice = true;
        }
        else if (_currentDialogue.Choices.Count > 1)
        {
            _isSingleChoice = false;
            CreateButtons();
        }
        StartCoroutine(ScrollDown());
    }
    private void ExposedProperties()
    {
        if (_currentDialogue.ExposedProperties.Count > 0)
        {

            bool isTrue = false;
            foreach (DSExposedProperty property in _currentDialogue.ExposedProperties)
            {
                var item = _allExposedProperties.Find(x => x.Name == property.Name);
                int index = _allExposedProperties.IndexOf(item);

                if (_currentDialogue.DialogueType == DSDialogueType.IfOneTrue)
                {
                    if (_allExposedProperties[index].Value)
                    {
                        isTrue = true;
                        break;
                    }
                }
                else if (_currentDialogue.DialogueType == DSDialogueType.IfAllTrue)
                {
                    isTrue = true;

                    if (!_allExposedProperties[index].Value)
                    {
                        isTrue = false;
                        break;
                    }
                }
                else
                {
                    // cos tu jest nie tak 

                    _allExposedProperties[index].Value = property.Value;
                    continue;
                }
            }
            if (_currentDialogue.DialogueType == DSDialogueType.IfOneTrue ||
                _currentDialogue.DialogueType == DSDialogueType.IfAllTrue)
            {
                if (isTrue)
                {
                    _currentDialogue = _currentDialogue.Choices[0].NextDialogue;
                }
                else
                {
                    _currentDialogue = _currentDialogue.Choices[1].NextDialogue;
                }
                ExposedProperties();
            }
        }
    }
    private void OnOptionChosen(int choiceIndex = 0)
    {
        _isSingleChoice = false;
        RemoveButonsFromContainer();
        _currentDialogue.Choices[choiceIndex].WasDisplayed = true;
        DSDialogueSO nextDialogue = _currentDialogue.Choices[choiceIndex].NextDialogue;

        if (nextDialogue == null)
        {
            CreateQuitButton();
            return; // No more dialogues to show, do whatever you want, like setting the currentDialogue to the startingDialogue
        }

        _currentDialogue = nextDialogue;
        ExposedProperties();

        ShowText();
    }
    private void QuitOption(GameObject quitButton)
    {
        _containerUI.SetActive(false);
        _textUI.text = string.Empty;
        _inputSystem.mouseInput.DialogueInputs.Disable();
        _inputSystem.mouseInput.MovementInputs.Enable();
        Destroy(quitButton);
    }
    /* Old if check
    private static DSDialogueSO DialogueFromIf(DSDialogueSO nextDialogue)
    {
        bool isTrue = false;

        if (nextDialogue.DialogueType == DSDialogueType.IfOneTrue)
        {
            foreach (var property in nextDialogue.ExposedProperties)
            {
                if (property.property.Value)
                {
                    isTrue = true;
                    break;
                }
            }

        }
        else if(nextDialogue.DialogueType == DSDialogueType.IfAllTrue)
        {
            isTrue = true;
            foreach (var property in nextDialogue.ExposedProperties)
            {
                if (!property.property.Value)
                {
                    isTrue = false;
                    break;
                }
            }
        }
        if (isTrue)
        {
            nextDialogue = nextDialogue.Choices[0].NextDialogue;
        }
        else
        {
            nextDialogue = nextDialogue.Choices[1].NextDialogue;
        }
        return nextDialogue;
    }*/
    #region Utilities

    private IEnumerator ScrollDown()
    {
        yield return new WaitForEndOfFrame();
        _scrollRect.verticalNormalizedPosition = 0;
    }
    private void RemoveButonsFromContainer()
    {
        if (_buttonList.Count >= 1)
        {
            foreach (GameObject button in _buttonList)
            {
                Destroy(button);
            }
            _buttonList.Clear();
        }
    }
    private void CreateButtons()
    {
        foreach (DSDialogueChoiceData dialogueSO in _currentDialogue.Choices)
        {
            GameObject button = Instantiate(_buttonPrefab, _contentUI.transform);
            DialogueButton dButton = button.GetComponent<DialogueButton>();
            dButton.text.text = dialogueSO.Text;
            dButton.choiceNumber = _currentDialogue.Choices.IndexOf(dialogueSO);
            dButton.button.onClick.AddListener(() =>
            {
                AddButtonTextToContainer(dialogueSO.Text);
                OnOptionChosen(dButton.choiceNumber);
            });
            if (dialogueSO.WasDisplayed)
            {
                var colors = dButton.button.colors;
                colors.normalColor = dButton.button.colors.disabledColor;
                dButton.button.colors = colors;
            }
            _buttonList.Add(button);
        }
    }
    private void CreateQuitButton()
    {
        GameObject button = Instantiate(_buttonPrefab, _contentUI.transform);
        DialogueButton dButton = button.GetComponent<DialogueButton>();
        dButton.text.text = "Quit";
        dButton.button.onClick.AddListener(() =>
        {
            QuitOption(button);
        });
        _buttonList.Add(button);
        StartCoroutine(ScrollDown());
    }
    private void AddButtonTextToContainer(string text)
    {
        _textUI.text += text;
        _textUI.text += "\n";
        _textUI.text += "\n";
    }
    private void OnLeftClik()
    {
        if (_isSingleChoice)
        {
            OnOptionChosen();
        }
    }
    #endregion
}
