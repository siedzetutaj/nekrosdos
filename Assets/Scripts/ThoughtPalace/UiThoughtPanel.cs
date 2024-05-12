using System;
using System.Linq;
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
    [SerializeField] public TPAllConnectionsSO ThoughtConnections;

    private void Start()
    {
        ThoughtConnections.AllConnections = ThoughtConnections.AllConnections.Where(item => item != null).ToList();
    }
}