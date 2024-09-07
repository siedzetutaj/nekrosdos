using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.InputSystem;
using System.Linq;
public class InformationController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{

    [SerializeField] public SerializableGuid ThoughtNodeGuid;
    [SerializeField] public RectTransform recTransform;

    [SerializeField] private GameObject _thoughtToCopy;

    private Camera _mainCamera;
    private GameObject _draggedThought;
    //bool referes to if the one is first = true, and last =  false
    public SerializableDictionary<LineController,bool> LineRenderers = new SerializableDictionary<LineController, bool>();

    private bool _isInInformation = true;

    [NonSerialized] public TextMeshProUGUI DescriptionTMP;
    [NonSerialized] public string Description;
    [NonSerialized] public TPThoughtSO Thought;
    [NonSerialized] public UiThoughtPanel ThoughtPanel;
    [NonSerialized] public Transform DraggedParent;
    [NonSerialized] public UIInformationDisplay InformationDisplay;
    private void OnEnable()
    {
        
    }
    public void Initialize(TPThoughtSO thought, TextMeshProUGUI descriptionTMP, Transform draggedParent, UiThoughtPanel thoughtPanel, UIInformationDisplay informationDisplay, Camera mainCamera)
    {
        Thought = thought;
        Description = thought.Description;
        DescriptionTMP = descriptionTMP;
        ThoughtPanel = thoughtPanel;
        DraggedParent = draggedParent;
        InformationDisplay = informationDisplay;
        _mainCamera = mainCamera;
    }
    #region Pointer
    public void OnPointerDown(PointerEventData eventData)
    {
        bool leftClick = eventData.button == PointerEventData.InputButton.Left;
        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        bool middleClick = eventData.button == PointerEventData.InputButton.Middle;

        #region LeftClick
        if (_isInInformation && leftClick && !ThoughtPanel.isCreatingLine)
        {
            CreateThought(eventData);
        }
        else if (leftClick)
        {
            StartDraggingThought(eventData);
        }
        #endregion
        #region Right Click
        else if (!_isInInformation && !ThoughtPanel.isCreatingLine && rightClick && !ThoughtPanel.isDraggingThought)
        {
            var nodesInGroup = ThoughtPanel.GetNodesInGroup(ThoughtNodeGuid);
            Debug.Log($"Nody w tej samej grupie co {ThoughtNodeGuid}");
            foreach (var nodeId in nodesInGroup)
            {
                Debug.Log(nodeId);
            }
            CreateBeginigLinePoint();
        }
        #endregion
        else if (middleClick && !_isInInformation && !ThoughtPanel.isCreatingLine)
        {
            DelateThought();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        bool leftClick = eventData.button == PointerEventData.InputButton.Left;
        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        
        if (leftClick)
        {
            DragThought(eventData);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        bool leftClick = eventData.button == PointerEventData.InputButton.Left;
        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        //LeftClick
        if (_isInInformation && leftClick)
        {
            EndDrag();
        }
        else if (!_isInInformation && leftClick)
        {
            ThoughtPanel.isDraggingThought = false;
        }
        //RightClick
        else if (!_isInInformation && ThoughtPanel.isCreatingLine && rightClick
                 && ThoughtPanel.firstThoughToConnect != this)
        {
            CreateEndLinePoint();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isInInformation)
        {
            SetActiveDescription(InformationDisplay.isDraggingThought);
        }
        else
        {
            SetActiveDescription(ThoughtPanel.isDraggingThought);
        }
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (_isInInformation)
        {
            DispalyDescrition(InformationDisplay.isDraggingThought);
        }
        else
        {
            DispalyDescrition(ThoughtPanel.isDraggingThought);
        }
    }
    public void OnPointerExit(PointerEventData eventData = null)
    {
        if (_isInInformation)
        {
            DisableDescription(InformationDisplay.isDraggingThought);
        }
        else
        {
            DisableDescription(ThoughtPanel.isDraggingThought);
        }
    }
    #endregion
    #region Creations
    private void CreateThought(PointerEventData eventData)
    {
        _draggedThought = Instantiate(_thoughtToCopy, DraggedParent);
        SetRectTransformToMiddle(_draggedThought);
        InformationController draggedThoughtController = _draggedThought.GetComponent<InformationController>();
        draggedThoughtController.Initialize(Thought, ThoughtPanel.descriptionTMP, DraggedParent, ThoughtPanel, InformationDisplay, _mainCamera);
        draggedThoughtController._isInInformation = false;
        draggedThoughtController.ThoughtNodeGuid = Guid.NewGuid();
        ThoughtPanel.AddNode(draggedThoughtController.ThoughtNodeGuid, Thought.ID);
        ThoughtPanel.allCreatedThoughts.Add(draggedThoughtController.ThoughtNodeGuid,draggedThoughtController);
        OnPointerExit();
        InformationDisplay.isDraggingThought = true;
        Vector3 thoughtPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        thoughtPos.z = -5;
        _draggedThought.transform.position = thoughtPos;
    }
    public void LoadThought(ThoughtSaveData thoughtSaveData)
    {
        SetRectTransformToMiddle(gameObject);
        gameObject.GetComponent<RectTransform>().anchoredPosition = thoughtSaveData.Position;
        _isInInformation = false;
        ThoughtNodeGuid = thoughtSaveData.Guid;
        ThoughtPanel.AddNode(ThoughtNodeGuid, Thought.ID);
        ThoughtPanel.allCreatedThoughts.Add(ThoughtNodeGuid, this);
    }
    private void CreateBeginigLinePoint()
    {
        GameObject go = Instantiate(ThoughtPanel.linePrefab, ThoughtPanel.LineHolder);
        ThoughtPanel.firstThoughToConnect = this;
        ThoughtPanel.FistID = ThoughtNodeGuid;
        var lineController = go.GetComponent<LineController>();
        ThoughtPanel.activeLineController = lineController;
        lineController.ChangePointPosition(0, recTransform.anchoredPosition);
        LineRenderers.Add(lineController, true);
        ThoughtPanel.isCreatingLine = true;
    }
    private void CreateEndLinePoint()
    {
        LineRenderers.Add(ThoughtPanel.activeLineController, false);
        ThoughtPanel.AddConnection(ThoughtNodeGuid, recTransform.anchoredPosition,this);
    }
    #endregion
    #region Delating
    private void DelateThought()
    {
        ThoughtPanel.DeleateThought(this);
    }
    #endregion
    #region Drag
    private void StartDraggingThought(PointerEventData eventData)
    {
        ThoughtPanel.isDraggingThought = true;
        _draggedThought = gameObject;
        DragThought(eventData);
    }
    private void DragThought(PointerEventData eventData)
    {
        Vector3 thoughtPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        thoughtPos.z = -5;
        _draggedThought.transform.position = thoughtPos;
        if (LineRenderers.Count > 0)
        {
            foreach (var lineRenderer in LineRenderers)
            {
                if (lineRenderer.Value)
                {
                    lineRenderer.Key.ChangePointPosition(0, recTransform.anchoredPosition);
                }
                else if (!lineRenderer.Value)
                {
                    lineRenderer.Key.ChangePointPosition(1, recTransform.anchoredPosition);
                }
            }
        }
    }
    private void EndDrag()
    {
        _draggedThought.transform.SetParent(ThoughtPanel.ThoughtHolderTransform);
        _draggedThought.GetComponent<InformationController>()._isInInformation = false;
        InformationDisplay.isDraggingThought = false;
        ThoughtPanel.isDraggingThought = false;
    }
    #endregion
    #region Description
    private void SetActiveDescription(bool isDragged)
    {
        if (!isDragged)
        {
            DescriptionTMP.text = Description;
            DescriptionTMP.gameObject.SetActive(true);
        }
    }
    private void DispalyDescrition(bool isDragged)
    {
        if (!isDragged)
        {
            Vector2 mousePos = InputSystem.Instance.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
            Vector2 UISize = DescriptionTMP.rectTransform.sizeDelta;
            Vector3 topLeftCorner = new Vector3(mousePos.x + UISize.x / 2 + 10, mousePos.y - UISize.y / 2, DescriptionTMP.transform.position.z);
            DescriptionTMP.transform.position = topLeftCorner;
        }
    }
    private void DisableDescription(bool isDragged)
    {
        if (!isDragged)
        {
            DescriptionTMP.gameObject.SetActive(false);
        }
    }
    #endregion
    #region Utilites
    private void SetRectTransformToMiddle(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0.5f, 0.5f);
        uitransform.anchorMax = new Vector2(0.5f, 0.5f);
        uitransform.pivot = new Vector2(0.5f, 0.5f);
    }
    #endregion
}
