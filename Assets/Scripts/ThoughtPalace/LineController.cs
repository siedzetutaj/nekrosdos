using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class LineController : MonoBehaviour
{
    public bool IsDraggedByMouse = true;
    public ConnectedThoughtsGuid connectionGuids;
    
    [SerializeField] private UILineRenderer lineRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider2D;

    private InputSystem inputSystem;
    private void Awake()
    {
        inputSystem = FindObjectOfType<InputSystem>();
        lineRenderer = GetComponent<UILineRenderer>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }
    private void Update()
    {
        ChangeEndPointToMousePosition();
    }

    public void ChangePointPosition(int point, Vector2 pos)
    {
        lineRenderer.Points[point] = pos;
        lineRenderer.SetAllDirty();
        UpdateCollider();
    }
    private void ChangeEndPointToMousePosition()
    {
        if (IsDraggedByMouse)
        {
            //calculate mouse position
            Vector2 mousePosCenter = inputSystem.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
            mousePosCenter.x -= Screen.width / 2f;
            mousePosCenter.y -= Screen.height / 2f;
            ChangePointPosition(1, mousePosCenter);
        }
    }
    public void UpdateCollider()
    {
        if (lineRenderer.Points == null || lineRenderer.Points.Length < 2)
        {
            return;
        }

        List<Vector2> colliderPoints = new List<Vector2>();
        float halfThickness = lineRenderer.LineThickness / 2f;

        for (int i = 0; i < lineRenderer.Points.Length - 1; i++)
        {
            Vector2 start = lineRenderer.Points[i];
            Vector2 end = lineRenderer.Points[i + 1];
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x);

            colliderPoints.Add(start + normal * halfThickness);
            colliderPoints.Add(start - normal * halfThickness);
            colliderPoints.Add(end - normal * halfThickness);
            colliderPoints.Add(end + normal * halfThickness);
        }

        polygonCollider2D.SetPath(0, colliderPoints.ToArray());
    }
}
