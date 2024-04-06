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
    public void Initialize(TPThoughtSO thought,TextMeshProUGUI description)
    {
        Image.sprite = thought.Sprite;
        InformationName.text = thought.Name;
        MyThought = thought;
        Information.description = thought.Description;
        Information.thought = MyThought;
        Information.descriptionTMP = description;
}
}
