using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.InputSystem;
public class InformationInInformationPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{

    [SerializeField] private GameObject thoughtToCopy;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private RectTransform recTransform;

    private GameObject draggedThought;
    //bool referes to if the one is first = true, and last =  false
    private Dictionary<LineController,bool> lineRenderers = new Dictionary<LineController, bool>();

    private bool IsInInformation = true;

    [NonSerialized] public TextMeshProUGUI DescriptionTMP;
    [NonSerialized] public string Description;
    [NonSerialized] public TPThoughtSO Thought;
    [NonSerialized] public UiThoughtPanel ThoughtPanel;
    [NonSerialized] public Transform DraggedParent;
    [NonSerialized] public UIInformationDisplay InformationDisplay;
    public void OnPointerDown(PointerEventData eventData)
    {
        #region LeftClick
        if (IsInInformation && eventData.button == PointerEventData.InputButton.Left)
        {
            CreateThought();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            draggedThought = gameObject;
            draggedThought.transform.position = eventData.position;
        }
        #endregion
        #region Right Click
        else if (!IsInInformation && !ThoughtPanel.isCreatingLine && eventData.button == PointerEventData.InputButton.Right)
        {
            CreateBeginigLinePoint();
        }
        //else if (!IsInInformation && IsCreatingLine && eventData.button == PointerEventData.InputButton.Right
        //         && ThoughtPanel.activeThough != this.gameObject)
        //{
        //    CreateEndLinePoint();
        //}
        #endregion
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            draggedThought.transform.position = eventData.position;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsInInformation && eventData.button == PointerEventData.InputButton.Left)
        {
            draggedThought.transform.SetParent(ThoughtPanel.ThoughtPanelTransform);
            draggedThought.GetComponent<InformationInInformationPanel>().IsInInformation = false;
            InformationDisplay.isBeingDragged = false;
        }
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
            if (!InformationDisplay.isBeingDragged)
            {
                DescriptionTMP.text = Description;
                DescriptionTMP.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!ThoughtPanel.isBeingDragged)
            {
                DescriptionTMP.text = Description;
                DescriptionTMP.gameObject.SetActive(true);
            }
        }
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (IsInInformation)
        {
            if (!InformationDisplay.isBeingDragged)
            {
                Vector2 mousePos = InputSystem.Instance.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
                Vector2 UISize = DescriptionTMP.rectTransform.sizeDelta;
                Vector3 topLeftCorner = new Vector3(mousePos.x + UISize.x / 2 + 10, mousePos.y - UISize.y / 2, DescriptionTMP.transform.position.z);
                DescriptionTMP.transform.position = topLeftCorner;
            }
        }
        else
        {
            if (!ThoughtPanel.isBeingDragged)
            {
                Vector2 mousePos = InputSystem.Instance.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
                Vector2 UISize = DescriptionTMP.rectTransform.sizeDelta;
                Vector3 topLeftCorner = new Vector3(mousePos.x + UISize.x / 2 + 10, mousePos.y - UISize.y / 2, DescriptionTMP.transform.position.z);
                DescriptionTMP.transform.position = topLeftCorner;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData = null)
    {
        if (IsInInformation)
        {
            if (!InformationDisplay.isBeingDragged)
            {
                DescriptionTMP.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!ThoughtPanel.isBeingDragged)
            {
                DescriptionTMP.gameObject.SetActive(false);
            }
        }
    }
    public void Initialize(TPThoughtSO thought, TextMeshProUGUI descriptionTMP, Transform draggedParent, UiThoughtPanel thoughtPanel, UIInformationDisplay informationDisplay)
    {
        Thought = thought;
        Description = thought.Description;
        DescriptionTMP = descriptionTMP;
        ThoughtPanel = thoughtPanel;
        DraggedParent = draggedParent;
        InformationDisplay = informationDisplay;
    }
    private void CreateThought()
    {
        draggedThought = Instantiate(thoughtToCopy, DraggedParent);
        draggedThought.transform.position = transform.position;
        SetRectTransformToMiddle(draggedThought);
        draggedThought.GetComponent<InformationInInformationPanel>().Initialize(Thought, ThoughtPanel.descriptionTMP, DraggedParent, ThoughtPanel, InformationDisplay);
        draggedThought.GetComponent<InformationInInformationPanel>().IsInInformation = false;
        OnPointerExit();
        InformationDisplay.isBeingDragged = true;
    }
    private void CreateBeginigLinePoint()
    {
        GameObject go = Instantiate(linePrefab, ThoughtPanel.LineHolder);
        ThoughtPanel.activeThough = this.gameObject;
        var lineController = go.GetComponent<LineController>();
        ThoughtPanel.activeLineController = lineController;
        lineController.SetPosioton(0, recTransform.anchoredPosition);
        lineRenderers.Add(lineController, true);
        ThoughtPanel.isCreatingLine = true;
    }
    private void CreateEndLinePoint()
    {
        ThoughtPanel.activeLineController.IsDraggedByMouse = false;
        ThoughtPanel.activeLineController.SetPosioton(1, recTransform.anchoredPosition);
        lineRenderers.Add(ThoughtPanel.activeLineController, false);
        ThoughtPanel.activeLineController = null;
        ThoughtPanel.activeThough = null;
        ThoughtPanel.isCreatingLine = false;
    }
    private void SetRectTransformToMiddle(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0.5f, 0.5f);
        uitransform.anchorMax = new Vector2(0.5f, 0.5f);
        uitransform.pivot = new Vector2(0.5f, 0.5f);
    }
}
