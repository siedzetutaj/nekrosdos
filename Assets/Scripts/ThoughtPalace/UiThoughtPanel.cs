using System;
using TMPro;
using UnityEngine;

public class UiThoughtPanel : MonoBehaviour
{
    [NonSerialized] public bool isBeingDragged;
    [NonSerialized] public bool isCreatingLine = false;
    [NonSerialized] public LineController activeLineController;
    [NonSerialized] public GameObject activeThough; //one that currently creates line

    [SerializeField] public TextMeshProUGUI descriptionTMP;
    [SerializeField] public RectTransform ThoughtPanelTransform;
    [SerializeField] public RectTransform LineHolder;
}