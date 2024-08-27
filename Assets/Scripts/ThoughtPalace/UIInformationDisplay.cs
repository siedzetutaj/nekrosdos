using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
public class UIInformationDisplay : MonoBehaviourSingleton<UIInformationDisplay>
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
    [SerializeField] private List<InformationPrefabData> AllUnlockedThoughts = new();
    

    public void Initialize()
    {
        _mainCamera = Camera.main;
    }

    public void Start()
    {
        _mainCamera = Camera.main;
#if UNITY_EDITOR
        foreach (TPThoughtSO thought in _allThoughtsSO.AllThoughts)
        {
            GameObject Information = Instantiate(_informationPrefab, _content);
            InformationPrefabData informationData = Information.GetComponent<InformationPrefabData>();
            informationData.Initialize(thought, _descriptionTMP, _draggedParent, _thoughtPanel, this, _mainCamera);
        }
#endif
    }
    public void UnlockThoughtInTPPanel(TPThoughtSO thought)
    {
        if (!AllUnlockedThoughts.Any(x => x.MyThought == thought))
        {
            GameObject Information = Instantiate(_informationPrefab, _content);
            InformationPrefabData informationData = Information.GetComponent<InformationPrefabData>();
            AllUnlockedThoughts.Add(informationData);
            informationData.Initialize(thought, _descriptionTMP, _draggedParent, _thoughtPanel, this, _mainCamera);
        }
    }
}
