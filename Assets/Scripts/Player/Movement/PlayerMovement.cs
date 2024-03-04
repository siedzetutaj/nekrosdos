using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviourSingleton<PlayerMovement>
{
    [NonSerialized] public Vector2 destination;

    [SerializeField] private InputSystem _inputs;
    [SerializeField] private float _speed;
    [SerializeField] private Transform _body;
    [SerializeField] private CircleCollider2D _bodyCollider;
    
    public Tilemap map;

    private LayerMask _interactionMask;

    private void Start()
    {
        _interactionMask = LayerMask.GetMask("Interactable");
        destination = transform.position;
    }
    private void Update()
    {
        if(_inputs.holdLeft)
        {
            Move();
        }
        if (Vector2.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * _speed);
            LookAtPosition();
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
    private void LookAtPosition()
    {
        _body.Rotate(0, 0.2f, 0, Space.Self);
    }
    public void MoveToInteractable(Vector2 target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target, 300, _interactionMask);

        Debug.DrawLine(transform.position, hit.point, Color.red);

        if (hit.collider != null)
        {
            destination = hit.point;
        }
        else
        {
            destination = target;
        }
    }
}
