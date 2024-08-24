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
    [SerializeField] private Transform _body;
    [SerializeField] private Animator _animator;
    

    public Tilemap map;

    private bool _isGointToHaveInteraction = false;
    private DSDialogue _dialogue;
    private bool _isPused = false;
    private Quaternion _initialRotation;
    private int _animationSpeed = 5;

    public void Initialize()
    {
        _thoughtPalaceUI = UIInformationHolder.Instance.ThoughtPalaceUI;
    }
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
        _initialRotation = _body.rotation;
        destination = transform.position;
        _agent.SetDestination(transform.position);
        map = FindObjectOfType<Tilemap>();
        _dialogueDisplay = DialogueDisplay.Instance;
    }
    private void Update()
    {

        if (!_isPused)
        {
            Movement();
        }
        else
        {
            _animator.speed = 1;
            _agent.SetDestination(transform.position);
            _animator.SetBool("IsMoving", false);
        }

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            StopMovementAnimation();
        }
        else
        {
            BeginAnimationAndSetRotation();
        }

    }
    #region Animating
    private void BeginAnimationAndSetRotation()
    {
        if (_agent.velocity.sqrMagnitude > 0)
        {
            _animator.SetBool("IsMoving", true);
            _animator.speed = _agent.velocity.magnitude/_animationSpeed;
            SetDirection();

        }
    }

    private void StopMovementAnimation()
    {
        if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
        {
            _animator.speed = 1;
            _animator.SetBool("IsMoving", false);
        }
    }
    #endregion
    private void SetDirection()
    {
        if (Vector2.Distance(_agent.destination, _body.position) >= 0.01)
        {
            var direction = _agent.destination - _body.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            _body.rotation = _initialRotation * targetRotation;
        }
    }

    #region Movement
    private void MoveToMouse()
    {
        _isGointToHaveInteraction = false;
        Vector2 mousePosition = _inputSystem.mouseInput.MovementInputs.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPostion = map.WorldToCell(mousePosition);
        if (map.HasTile(gridPostion))
        {
            destination = mousePosition;
        }
    }
    private void Movement()
    {
        if (_isGointToHaveInteraction && Vector2.Distance(transform.position, _agent.destination) < 0.1f)
        {
            DisplayDialogue();

            _isGointToHaveInteraction = false;
            _dialogue = null;
        }
        if (_inputSystem.holdLeft)
        {
            MoveToMouse();
        }
        _agent.SetDestination(destination);
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
            _isPused = true;
        }
        else
        {
            _thoughtPalaceUI.SetActive(false);
            _inputSystem.mouseInput.MovementInputs.Enable();
            _inputSystem.mouseInput.TPInputs.Disable();
            _isPused = false;
        }
    }
    #endregion
}