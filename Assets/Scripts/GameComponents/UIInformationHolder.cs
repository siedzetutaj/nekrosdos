using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInformationHolder : MonoBehaviourSingleton<UIInformationHolder>
{
    public GameObject ThoughtPalaceUI;
    public GameObject DialogueScrollViewUI;
    public GameObject DialogueContinerUI;
    public GameObject DialogueContentUI;
    public TextMeshProUGUI DialogueTextUI;

    private void OnEnable()
    {
        DialogueContinerUI.SetActive(false);
    }

}
