using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviourSingleton<PlayerController>
{
    [NonSerialized] public Vector2 destination;

    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private DialogueDisplay _dialogueDisplay;
    [SerializeField] private GameObject _thoughtPalaceUI;

    public Tilemap map;

    private bool _isGointToHaveInteraction = false;
    private DSDialogue _dialogue;

    private void OnEnable()
    {
        _inputSystem.ToggleTP += ToggleThoughtpalace;
    }
    private void OnDisable()
    {
        _inputSystem.ToggleTP -= ToggleThoughtpalace;

    }
    private void Awake()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
    private void Start()
    {
        destination = transform.position;
    }
    private void Update()
    {
        if (_isGointToHaveInteraction && Vector2.Distance(transform.position, _agent.destination) < 0.1f)
        {
            DisplayDialogue();

            _isGointToHaveInteraction = false;
            _dialogue = null;
        }
        if (_inputSystem.holdLeft)
        {
            Move();
        }
        _agent.SetDestination(destination);
    }
    #region Movement
    private void Move()
    {
        _isGointToHaveInteraction = false;
        Vector2 mousePosition = _inputSystem.mouseInput.MovementInputs.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPostion = map.WorldToCell(mousePosition);
        if (map.HasTile(gridPostion))
        {
            destination = mousePosition;
            // transform.LookAt(mousePosition);
        }
    }
    #endregion
    #region Interactions
    public void MoveToInteractable(Vector2 target)
    {
        destination = target;
    }
    public void SetDialogue(DSDialogue dialogue)
    {
        _dialogue = dialogue;
        _isGointToHaveInteraction = true;
    }
    private void DisplayDialogue()
    {
        _inputSystem.mouseInput.MovementInputs.Disable();
        _dialogueDisplay.startingDialogue = _dialogue;
        _dialogueDisplay.StartDisplaying();
    }
    #endregion
    #region ThoughtPalace
    private void ToggleThoughtpalace()
    {
        if (!_thoughtPalaceUI.activeSelf)
        {
            _thoughtPalaceUI.SetActive(true);
            _inputSystem.mouseInput.MovementInputs.Disable();
            _inputSystem.mouseInput.TPInputs.Enable();
        }
        else
        {
            _thoughtPalaceUI.SetActive(false);
            _inputSystem.mouseInput.MovementInputs.Enable();
            _inputSystem.mouseInput.TPInputs.Disable();
        }
    }
    #endregion
}