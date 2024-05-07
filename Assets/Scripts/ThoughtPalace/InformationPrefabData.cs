using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InformationPrefabData : MonoBehaviour
{
    [SerializeField] private Image Image;
    [SerializeField] private TextMeshProUGUI InformationName;
    [SerializeField] private TPThoughtSO MyThought;
    [SerializeField] private InformationInInformationPanel Information;
    public void Initialize(TPThoughtSO thought,TextMeshProUGUI descriptionTMP, Transform draggedParent, UiThoughtPanel thoughtPanel, UIInformationDisplay informationDisplay)
    {
        Image.sprite = thought.Sprite;
        InformationName.text = thought.Name;
        MyThought = thought;
        Information.Description = thought.Description;
        Information.Thought = MyThought;
        Information.DescriptionTMP = descriptionTMP;
        Information.ThoughtPanel = thoughtPanel;
        Information.DraggedParent = draggedParent;
        Information.InformationDisplay = informationDisplay;
}
}
