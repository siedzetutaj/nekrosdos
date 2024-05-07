using TMPro;
using UnityEngine;
public class UIInformationDisplay : MonoBehaviour
{
    //var for each information to see if any is being dragged
    public bool isBeingDragged;

    [SerializeField] private TPAllThoughtsSO AllThoughtsSO;
    [SerializeField] private GameObject InformationPrefab;
    [SerializeField] private Transform Content;
    [SerializeField] private TextMeshProUGUI descriptionTMP;
    [SerializeField] private UiThoughtPanel ThoughtPanel;
    [SerializeField] private Transform DraggedParent; 

    public void Start()
    {
        foreach(TPThoughtSO thought in AllThoughtsSO.AllThoughts)
        {
            GameObject Information = Instantiate(InformationPrefab, Content);
            InformationPrefabData informationData = Information.GetComponent<InformationPrefabData>();
            informationData.Initialize(thought, descriptionTMP, DraggedParent, ThoughtPanel, this);
        }
    }
}
