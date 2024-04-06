using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
public class InformationInInformationPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{

    [SerializeField] private GameObject thoughtToCopy;
    
    private GameObject draggedThought;

    [NonSerialized] public TextMeshProUGUI descriptionTMP;
    [NonSerialized] public string description;
    [NonSerialized] public TPThoughtSO thought;

    public void OnPointerDown(PointerEventData eventData)
    {
        draggedThought = Instantiate(thoughtToCopy, transform.parent);
        draggedThought.transform.position = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggedThought.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // When dragging ends, if over a drop target, perform drop action
        // You can implement your drop logic here
        // For simplicity, let's just destroy the copy
        Destroy(draggedThought);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Display the object's name when the mouse hovers over it
        descriptionTMP.text = gameObject.name;
        descriptionTMP.gameObject.SetActive(true);
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        descriptionTMP.transform.position = Input.mousePosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide the hover text when the mouse exits the object
        descriptionTMP.gameObject.SetActive(false);
    }


}
