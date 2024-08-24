using TMPro;
using UnityEngine;
public class UIInformationDisplay : MonoBehaviour
{
    //var for each information to see if any is being dragged
    public bool isDraggingThought;

    [SerializeField] private TPAllThoughtsSO _allThoughtsSO;
    [SerializeField] private GameObject _informationPrefab;
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private UiThoughtPanel _thoughtPanel;
    [SerializeField] private Transform _draggedParent;
    [SerializeField] private Camera _mainCamera;

    public void Start()
    {
        _mainCamera = Camera.main;
        foreach (TPThoughtSO thought in _allThoughtsSO.AllThoughts)
        {
            GameObject Information = Instantiate(_informationPrefab, _content);
            InformationPrefabData informationData = Information.GetComponent<InformationPrefabData>();
            informationData.Initialize(thought, _descriptionTMP, _draggedParent, _thoughtPanel, this, _mainCamera);
        }
    }
}
