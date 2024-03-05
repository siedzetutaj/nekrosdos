using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviourSingleton<PlayerMovement>
{
    [NonSerialized] public Vector2 destination;

    [SerializeField] private InputSystem _inputs;
    [SerializeField] private NavMeshAgent _agent;

    public Tilemap map;

    private bool IsGointToHaveInteraction = false;

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
        _agent.SetDestination(destination);
        if (_inputs.holdLeft)
        {
            Move();
        }
        if (IsGointToHaveInteraction && Vector2.Distance(transform.position, _agent.destination) < 0.1f)
        {
            Debug.Log("hello");
            IsGointToHaveInteraction = false;
        }
    }
    private void Move()
    {
        Vector2 mousePosition = _inputs.mouseInput.MouseInputs.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPostion = map.WorldToCell(mousePosition);
        if (map.HasTile(gridPostion))
        {
            destination = mousePosition;
            // transform.LookAt(mousePosition);
        }
    }

    public void MoveToInteractable(Vector2 target)
    {
        destination = target;
        IsGointToHaveInteraction = true;

    }
}