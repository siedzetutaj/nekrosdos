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
    public Action onLeftClick;
    public Action onRightClick;
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
        mouseInput.MouseInputs.MouseLeftClick.performed -= LeftClick;
        mouseInput.MouseInputs.MouseRightClick.performed -= RightClick;
        mouseInput.Disable();
    }
    private void Start()
    {
        mouseInput.MouseInputs.MouseLeftClick.performed += LeftClick;
        mouseInput.MouseInputs.MouseRightClick.performed += RightClick;
        mouseInput.MouseInputs.MouseLeftClick.canceled += CancelLeftClick;
    }
    #endregion
    #region Actions
    private void RightClick(InputAction.CallbackContext context)
    {
        onRightClick?.Invoke();
    }
    private void LeftClick(InputAction.CallbackContext obj)
    {
        holdLeft = true;
        onLeftClick?.Invoke();
    }  

    private void CancelLeftClick(InputAction.CallbackContext obj)
    {
        holdLeft = false;
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