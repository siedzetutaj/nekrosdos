using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.InputSystem;
public class InformationInInformationPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{

    [SerializeField] private GameObject _thoughtToCopy;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private RectTransform _recTransform;

    private GameObject _draggedThought;
    //bool referes to if the one is first = true, and last =  false
    private Dictionary<LineController,bool> _lineRenderers = new Dictionary<LineController, bool>();

    private bool IsInInformation = true;

    [NonSerialized] public TextMeshProUGUI DescriptionTMP;
    [NonSerialized] public string Description;
    [NonSerialized] public TPThoughtSO Thought;
    [NonSerialized] public UiThoughtPanel ThoughtPanel;
    [NonSerialized] public Transform DraggedParent;
    [NonSerialized] public UIInformationDisplay InformationDisplay;
    public void Initialize(TPThoughtSO thought, TextMeshProUGUI descriptionTMP, Transform draggedParent, UiThoughtPanel thoughtPanel, UIInformationDisplay informationDisplay)
    {
        Thought = thought;
        Description = thought.Description;
        DescriptionTMP = descriptionTMP;
        ThoughtPanel = thoughtPanel;
        DraggedParent = draggedParent;
        InformationDisplay = informationDisplay;
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
            SetActiveDescription(InformationDisplay.isBeingDragged);
        }
        else
        {
            SetActiveDescription(ThoughtPanel.isBeingDragged);
        }
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (IsInInformation)
        {
            DispalyDescrition(InformationDisplay.isBeingDragged);
        }
        else
        {
            DispalyDescrition(ThoughtPanel.isBeingDragged);
        }
    }
    public void OnPointerExit(PointerEventData eventData = null)
    {
        if (IsInInformation)
        {
            DisableDescription(InformationDisplay.isBeingDragged);
        }
        else
        {
            DisableDescription(ThoughtPanel.isBeingDragged);
        }
    }
    #endregion
    #region Creations
    private void CreateThought()
    {
        _draggedThought = Instantiate(_thoughtToCopy, DraggedParent);
        _draggedThought.transform.position = transform.position;
        SetRectTransformToMiddle(_draggedThought);
        _draggedThought.GetComponent<InformationInInformationPanel>().Initialize(Thought, ThoughtPanel.descriptionTMP, DraggedParent, ThoughtPanel, InformationDisplay);
        _draggedThought.GetComponent<InformationInInformationPanel>().IsInInformation = false;
        OnPointerExit();
        InformationDisplay.isBeingDragged = true;
    }
    private void CreateBeginigLinePoint()
    {
        GameObject go = Instantiate(_linePrefab, ThoughtPanel.LineHolder);
        ThoughtPanel.activeThough = this.gameObject;
        var lineController = go.GetComponent<LineController>();
        ThoughtPanel.activeLineController = lineController;
        lineController.SetPointPosition(0, _recTransform.anchoredPosition);
        _lineRenderers.Add(lineController, true);
        ThoughtPanel.isCreatingLine = true;
    }
    private void CreateEndLinePoint()
    {
        ThoughtPanel.activeLineController.IsDraggedByMouse = false;
        ThoughtPanel.activeLineController.SetPointPosition(1, _recTransform.anchoredPosition);
        _lineRenderers.Add(ThoughtPanel.activeLineController, false);
        ThoughtPanel.activeLineController = null;
        ThoughtPanel.activeThough = null;
        ThoughtPanel.isCreatingLine = false;
    }
    #endregion
    #region Drag
    private void StartDraggingThought(PointerEventData eventData)
    {
        _draggedThought = gameObject;
        _draggedThought.transform.position = eventData.position;
    }
    private void DragThought(PointerEventData eventData)
    {
        _draggedThought.transform.position = eventData.position;
        if (_lineRenderers.Count > 0)
        {
            foreach (var lineRenderer in _lineRenderers)
            {
                if (lineRenderer.Value)
                {
                    lineRenderer.Key.SetPointPosition(0, _recTransform.anchoredPosition);
                }
                else if (!lineRenderer.Value)
                {
                    lineRenderer.Key.SetPointPosition(1, _recTransform.anchoredPosition);
                }
            }
        }
    }
    private void EndDrag()
    {
        _draggedThought.transform.SetParent(ThoughtPanel.ThoughtPanelTransform);
        _draggedThought.GetComponent<InformationInInformationPanel>().IsInInformation = false;
        InformationDisplay.isBeingDragged = false;
    }
    #endregion
    #region Description
    private void SetActiveDescription(bool isDragged)
    {
        if (isDragged)
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
