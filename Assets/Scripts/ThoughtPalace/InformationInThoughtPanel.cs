using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InformationInThoughtPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{

    private bool PlaceForTheFirstTime = true;

    [NonSerialized] public string Description;
    [NonSerialized] public TPThoughtSO Thought;
    [NonSerialized] public UiThoughtPanel ThoughtPanel;
    [NonSerialized] public UIInformationDisplay InformationDisplay;

    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlaceForTheFirstTime)
        {
            this.transform.SetParent(ThoughtPanel.transform, false);
            InformationDisplay.isBeingDragged = false;
            PlaceForTheFirstTime = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    public void OnPointerMove(PointerEventData eventData)
    {
    }
    public void OnPointerExit(PointerEventData eventData = null)
    {
    }
}
