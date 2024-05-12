using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviourSingleton<Interactor>
{
    [Header("Components")]
    [SerializeField] private InputSystem _inputs;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private SetCoursor _setCoursor;
    
    [Header("Variables")]
    [SerializeField] private float _interactionPointRadius;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private TextMeshProUGUI _displayName;

    [Header("Debug")]
    [SerializeField] private int _numfound;

    private readonly Collider2D[] _colliders = new Collider2D[3];
    private Vector2 _mousePosition;
    private Vector2 _worldPosition;
    private IInteractable currInteractable;
    #region setup
    private void OnEnable()
    {
        _inputs.onMovemenLeftClickUp += OnLeftClick;
    }
    private void OnDisable()
    {
        _inputs.onMovemenLeftClickUp -= OnLeftClick;
    }
    #endregion
    private void Update()
    {
        _mousePosition = _inputs.mouseInput.MovementInputs.MousePosition.ReadValue<Vector2>();
        _worldPosition = _mainCamera.ScreenToWorldPoint(_mousePosition);

        _numfound = Physics2D.OverlapCircleNonAlloc(_worldPosition, _interactionPointRadius, _colliders, _interactableMask);
        if (_numfound > 0)
        {
            _colliders[0].TryGetComponent(out IInteractable interactable);
            currInteractable = interactable;

            _setCoursor.SetCurosr(currInteractable.CursorType);

            _displayName.transform.position = _mousePosition;
            _displayName.text = currInteractable.InteractionName;
        }
        else if (_numfound == 0) 
        {
            _setCoursor.SetCurosr(CoursorType.arrow);

            _displayName.text = string.Empty;
            currInteractable = null;
        }
    }
    private void OnLeftClick()
    {
        if (currInteractable != null)
        {
            currInteractable.Interact(this);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_worldPosition, _interactionPointRadius);
    }
#endif
}
