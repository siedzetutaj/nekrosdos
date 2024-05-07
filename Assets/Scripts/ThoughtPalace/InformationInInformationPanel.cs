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

    private GameObject draggedThought;

    [NonSerialized] public TextMeshProUGUI DescriptionTMP;
    [NonSerialized] public string Description;
    [NonSerialized] public TPThoughtSO Thought;
    [NonSerialized] public UiThoughtPanel ThoughtPanel;
    [NonSerialized] public Transform DraggedParent;
    [NonSerialized] public UIInformationDisplay InformationDisplay;
    public void OnPointerDown(PointerEventData eventData)
    {
        draggedThought = Instantiate(thoughtToCopy, DraggedParent);
        draggedThought.transform.position = transform.position;
        draggedThought.GetComponent<InformationInInformationPanel>().enabled = false;
        
        var informationInThoughtPanel = draggedThought.GetComponent<InformationInThoughtPanel>();
        informationInThoughtPanel.enabled = true;   
        informationInThoughtPanel.ThoughtPanel = ThoughtPanel;
        informationInThoughtPanel.Description = Description;
        informationInThoughtPanel.Thought = Thought;
        informationInThoughtPanel.InformationDisplay = InformationDisplay;

        OnPointerExit();
        InformationDisplay.isBeingDragged = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        draggedThought.transform.position = eventData.position;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InformationDisplay.isBeingDragged)
        {
            DescriptionTMP.text = Description;
            DescriptionTMP.gameObject.SetActive(true);
        }
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (!InformationDisplay.isBeingDragged)
        {
            Vector2 mousePos = InputSystem.Instance.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
            Vector2 UISize = DescriptionTMP.rectTransform.sizeDelta;
            Vector3 topLeftCorner = new Vector3(mousePos.x + UISize.x / 2 + 10, mousePos.y - UISize.y / 2, DescriptionTMP.transform.position.z);
            DescriptionTMP.transform.position = topLeftCorner;
        }
    }
    public void OnPointerExit(PointerEventData eventData = null)
    {
        // Hide the hover text when the mouse exits the object
        if (!InformationDisplay.isBeingDragged)
        {
            DescriptionTMP.gameObject.SetActive(false);
        }
    }
}
