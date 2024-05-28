using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseRaycasterToLine : MonoBehaviour
{
    public GameObject dotPrefab; // Prefabrykat kropki
    [SerializeField] private Camera _uiCamera; 
    [SerializeField] private GraphicRaycaster _canvasGraphicRaycaster;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private LayerMask _uiLineLayerMask; 
    [SerializeField] private InputSystem _inputSystem;
    private void OnEnable()
    {
        _inputSystem.onTPLeftClickDown += OnMouseLeftClick;
    }

    private void OnDisable()
    {
        _inputSystem.onTPLeftClickDown += OnMouseLeftClick;
    }

    private void OnMouseLeftClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Konwertujemy pozycjê myszy z ekranu do œwiata 2D
        Vector2 worldPoint = _uiCamera.ScreenToWorldPoint(mousePosition);

        // Wykonujemy raycast 2D w kierunku pozycji myszy
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero, Mathf.Infinity, _uiLineLayerMask);
        ShowDot(worldPoint);
        
        // Sprawdzamy wyniki raycasta
        foreach (var hit in hits)
        {
            Debug.Log(hit);
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                break; // Przerywamy po pierwszym trafieniu
            }
        }
    }

    private void ShowDot(Vector2 position)
    {
        GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
        StartCoroutine(DestroyDotAfterTime(dot, 5f));
    }

    private IEnumerator DestroyDotAfterTime(GameObject dot, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(dot);
    }
}
