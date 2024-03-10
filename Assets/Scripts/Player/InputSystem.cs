using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviourSingleton<InputSystem>
{
    /* To add new input action
     * First add it in editor
     * Next make Action
     * Then Connect Action to input
     * Lastly create void that sends singal
     */
    public MouseInput mouseInput;

    #region Actions

    // Movement
    public Action onMovemenLeftClickDown;
    public Action onMovemenLeftClickUp;
    public Action onMovemenRightClickDown;
    
    // Dialogue
    public Action onDialogueLeftClickDown;
    #endregion
    #region Other variables
    public bool holdLeft = false;
    #endregion
    #region Assembly
    private void Awake()
    {
        mouseInput = new MouseInput();
    }
    private void OnEnable()
    {
        mouseInput.Enable();
    }
    #endregion
    #region Connect/Disconnect actions
    private void OnDisable()
    {
        mouseInput.MovementInputs.MouseLeftClick.started -= MovementLeftClickDown;
        mouseInput.MovementInputs.MouseLeftClick.canceled -= MovementLeftClickUp;
        mouseInput.MovementInputs.MouseRightClick.performed -= MovementRightClickDown;

        mouseInput.DialogueInputs.MouseLeftClick.started -= DialogueLeftClickDown;  

        mouseInput.Disable();
    }
    private void Start()
    {
        mouseInput.MovementInputs.MouseLeftClick.performed += MovementLeftClickDown;
        mouseInput.MovementInputs.MouseLeftClick.canceled += MovementLeftClickUp;
        mouseInput.MovementInputs.MouseRightClick.performed += MovementRightClickDown;

        mouseInput.DialogueInputs.MouseRightClick.started += DialogueLeftClickDown;
        mouseInput.DialogueInputs.Disable();
    }
    #endregion
    #region Movement Actions
    private void MovementRightClickDown(InputAction.CallbackContext context)
    {
        onMovemenRightClickDown?.Invoke();
    }
    private void MovementLeftClickDown(InputAction.CallbackContext obj)
    {
        holdLeft = true;
        onMovemenLeftClickDown?.Invoke();
    }  
    private void MovementLeftClickUp(InputAction.CallbackContext obj)
    {
        onMovemenLeftClickUp?.Invoke();
        holdLeft = false;
    }
    #endregion
    #region Dialogue Actions
    private void DialogueLeftClickDown(InputAction.CallbackContext obj)
    {
        onDialogueLeftClickDown?.Invoke();
    }
    #endregion
}
public interface IInputProvider
{
    PlayerInput GetInput();
}
[System.Serializable]
public struct PlayerInput
{
    public static PlayerInput None => new PlayerInput();
}