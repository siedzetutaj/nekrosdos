using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class UIInformationDisplay : MonoBehaviour
{
    [SerializeField] private TPAllThoughtsSO AllThoughtsSO;
    [SerializeField] private GameObject InformationPrefab;
    [SerializeField] private Transform Content;
    [SerializeField] private TextMeshProUGUI descriptionTMP;

    public void Start()
    {
        foreach(TPThoughtSO thought in AllThoughtsSO.AllThoughts)
        {
            GameObject Information = Instantiate(InformationPrefab, Content);
            InformationPrefabData informationData = Information.GetComponent<InformationPrefabData>();
            informationData.Initialize(thought, descriptionTMP);
        }
    }
}
