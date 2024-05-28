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

    [SerializeField] private GameObject _thoughtToCopy;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private RectTransform _recTransform;

    private Camera _mainCamera;
    private GameObject _draggedThought;
    //bool referes to if the one is first = true, and last =  false
    public SerializableDictionary<LineController,bool> LineRenderers = new SerializableDictionary<LineController, bool>();

    private bool IsInInformation = true;

    [NonSerialized] public TextMeshProUGUI DescriptionTMP;
    [NonSerialized] public string Description;
    [NonSerialized] public TPThoughtSO Thought;
    [NonSerialized] public UiThoughtPanel ThoughtPanel;
    [NonSerialized] public Transform DraggedParent;
    [NonSerialized] public UIInformationDisplay InformationDisplay;

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
        #region LeftClick
        if (IsInInformation && eventData.button == PointerEventData.InputButton.Left)
        {
            CreateThought();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartDraggingThought(eventData);
        }
        #endregion
        #region Right Click
        else if (!IsInInformation && !ThoughtPanel.isCreatingLine && eventData.button == PointerEventData.InputButton.Right)
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
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DragThought(eventData);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //LeftClick
        if (IsInInformation && eventData.button == PointerEventData.InputButton.Left)
        {
            EndDrag();
        }
        else if (!IsInInformation && eventData.button == PointerEventData.InputButton.Left)
        {
            ThoughtPanel.isDraggingThought = false;
        }
        //RightClick
        else if (!IsInInformation && ThoughtPanel.isCreatingLine && eventData.button == PointerEventData.InputButton.Right
                 && ThoughtPanel.activeThough != this.gameObject)
        {
            CreateEndLinePoint();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInInformation)
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
        if (IsInInformation)
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
        if (IsInInformation)
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
    private void CreateThought()
    {
        _draggedThought = Instantiate(_thoughtToCopy, DraggedParent);
        _draggedThought.transform.position = transform.position;
        SetRectTransformToMiddle(_draggedThought);
        InformationController draggedThoughtController = _draggedThought.GetComponent<InformationController>();
        draggedThoughtController.Initialize(Thought, ThoughtPanel.descriptionTMP, DraggedParent, ThoughtPanel, InformationDisplay, _mainCamera);
        draggedThoughtController.IsInInformation = false;
        draggedThoughtController.ThoughtNodeGuid = Guid.NewGuid();
        ThoughtPanel.AddNode(draggedThoughtController.ThoughtNodeGuid, Thought.ID);
        ThoughtPanel.createdThoughts.Add(draggedThoughtController);
        OnPointerExit();
        InformationDisplay.isDraggingThought = true;
    }
    private void CreateBeginigLinePoint()
    {
        GameObject go = Instantiate(_linePrefab, ThoughtPanel.LineHolder);
        ThoughtPanel.activeThough = this.gameObject;
        ThoughtPanel.FistID = ThoughtNodeGuid;
        var lineController = go.GetComponent<LineController>();
        ThoughtPanel.activeLineController = lineController;
        lineController.ChangePointPosition(0, _recTransform.anchoredPosition);
        LineRenderers.Add(lineController, true);
        ThoughtPanel.isCreatingLine = true;
    }
    private void CreateEndLinePoint()
    {
        ThoughtPanel.activeLineController.IsDraggedByMouse = false;
        ThoughtPanel.activeLineController.ChangePointPosition(1, _recTransform.anchoredPosition);
        ThoughtPanel.activeLineController.connectionGuids.Id1 = ThoughtPanel.FistID;
        ThoughtPanel.activeLineController.connectionGuids.Id2 = ThoughtNodeGuid;
        ThoughtPanel.activeLineController.UpdateCollider();
        LineRenderers.Add(ThoughtPanel.activeLineController, false);
        ThoughtPanel.AddConnection(ThoughtNodeGuid);
        ThoughtPanel.activeLineController = null;
        ThoughtPanel.activeThough = null;
        ThoughtPanel.isCreatingLine = false;
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
                    lineRenderer.Key.ChangePointPosition(0, _recTransform.anchoredPosition);
                }
                else if (!lineRenderer.Value)
                {
                    lineRenderer.Key.ChangePointPosition(1, _recTransform.anchoredPosition);
                }
            }
        }
    }
    private void EndDrag()
    {
        _draggedThought.transform.SetParent(ThoughtPanel.ThoughtPanelTransform);
        _draggedThought.GetComponent<InformationController>().IsInInformation = false;
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
