using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Interactor : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputSystem _inputs;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SetCoursor setCoursor;
    
    [Header("Variables")]
    [SerializeField] private float _interactionPointRadius;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private TextMeshProUGUI displayName;

    [Header("Debug")]
    [SerializeField] private int _numfound;

    private readonly Collider2D[] _colliders = new Collider2D[3];
    private Vector2 _mousePosition;
    private Vector2 _worldPosition;
    private IInteractable currInteractable;
    #region setup
    private void OnEnable()
    {
        _inputs.onRightClick += OnRightClick;
    }
    private void OnDisable()
    {
        _inputs.onRightClick -= OnRightClick;
    }
    #endregion
    private void Update()
    {
        _mousePosition = _inputs.mouseInput.MouseInputs.MousePosition.ReadValue<Vector2>();
        _worldPosition = mainCamera.ScreenToWorldPoint(_mousePosition);

        _numfound = Physics2D.OverlapCircleNonAlloc(_worldPosition, _interactionPointRadius, _colliders, _interactableMask);
        if (_numfound > 0)
        {
            _colliders[0].TryGetComponent(out IInteractable interactable);
            currInteractable = interactable;

            setCoursor.SetTexture(currInteractable.CursorType);

            displayName.transform.position = _mousePosition;
            displayName.text = currInteractable.InteractionPrompt;
        }
        else if (_numfound == 0) 
        {
            setCoursor.SetTexture(CoursorType.arrow);

            displayName.text = string.Empty;
            currInteractable = null;
        }
    }
    private void OnRightClick()
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
