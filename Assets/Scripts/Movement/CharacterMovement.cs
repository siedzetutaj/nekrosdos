using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    private MouseInput mouseInput;
    private Vector3 destination;

    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    
    public Tilemap map;
    private void Awake()
    {
        mouseInput = new MouseInput();
    }
    private void OnEnable()
    {
        mouseInput.Enable();
    }
    private void OnDisable()
    {
        mouseInput.Disable();
    }
    private void Start()
    {
        destination = transform.position;
        mouseInput.MouseInputs.MouseLeftClick.performed += x =>MouseClick();
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
            LookAtPosition();
        }
    }
    private void MouseClick()
    {
        Vector2 mousePosition = mouseInput.MouseInputs.MousePosition.ReadValue<Vector2>();
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

         transform.Rotate(0, 0.2f, 0, Space.Self);

    }
}
