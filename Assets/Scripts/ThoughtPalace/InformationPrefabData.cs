using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InformationPrefabData : MonoBehaviour
{
    public Image Image;
    public TextMeshProUGUI InformationName;
    public TPThoughtSO MyThought;
    public InformationController Information;
    public void Initialize(TPThoughtSO thought,TextMeshProUGUI descriptionTMP, Transform draggedParent, UiThoughtPanel thoughtPanel, UIInformationDisplay informationDisplay, Camera mainCamera)
    {
        Image.sprite = thought.Sprite;
        InformationName.text = thought.Name;
        MyThought = thought;
        Information.Initialize(thought, descriptionTMP, draggedParent, thoughtPanel, informationDisplay, mainCamera);
    }       
}
