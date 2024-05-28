using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class LineController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 clickPosition = eventData.position;

        // Przekonwertuj pozycj� klikni�cia myszy na lokalne wsp�rz�dne obiektu zawieraj�cego UILineRenderer
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRenderer.rectTransform, clickPosition, eventData.pressEventCamera, out Vector2 localClickPosition);

        // Sprawd�, czy klikni�cie nast�pi�o na linii
        bool isOnLine = IsPointOnLine(localClickPosition);

        if (isOnLine)
        {
            // Klikni�cie nast�pi�o na linii, wykonaj odpowiednie dzia�ania
            Debug.Log("linia!");
        }
    }
    private bool IsPointOnLine(Vector2 point)
    {
        // Sprawd�, czy punkt znajduje si� na kt�rej� z lini w UILineRenderer
        // Mo�esz u�y� swojej logiki do wykrywania, czy punkt znajduje si� na linii.
        // Na przyk�ad, mo�esz iterowa� po wszystkich segmentach linii i sprawdza�, czy punkt znajduje si� na kt�rym� z nich.
        // Mo�esz u�y� funkcji geometrii, aby to osi�gn��.

        // Poni�ej znajduje si� przyk�adowy kod, kt�ry sprawdza, czy punkt znajduje si� w odleg�o�ci mniejszej ni� tolerancja od linii:
        float tolerance = 10f; // Mo�esz dostosowa� tolerancj� do swoich potrzeb
        for (int i = 1; i < lineRenderer.Points.Length; i++)
        {
            Vector2 start = lineRenderer.Points[i - 1];
            Vector2 end = lineRenderer.Points[i];
            if (IsPointCloseToLine(point, start, end, tolerance))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsPointCloseToLine(Vector2 point, Vector2 start, Vector2 end, float tolerance)
    {
        // Oblicz odleg�o�� mi�dzy punktem a lini�
        float distance = DistancePointLine(point, start, end);

        // Sprawd�, czy odleg�o�� jest mniejsza ni� tolerancja
        return distance <= tolerance;
    }
    private float DistancePointLine(Vector2 point, Vector2 start, Vector2 end)
    {
        // Oblicz odleg�o�� punktu od linii za pomoc� r�wnania geometrycznego
        float numerator = Mathf.Abs((end.y - start.y) * point.x - (end.x - start.x) * point.y + end.x * start.y - end.y * start.x);
        float denominator = Mathf.Sqrt(Mathf.Pow(end.y - start.y, 2) + Mathf.Pow(end.x - start.x, 2));
        return numerator / denominator;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Vector2 clickPosition = eventData.position;

        // Przekonwertuj pozycj� klikni�cia myszy na lokalne wsp�rz�dne obiektu zawieraj�cego UILineRenderer
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRenderer.rectTransform, clickPosition, eventData.pressEventCamera, out Vector2 localClickPosition);

        // Sprawd�, czy klikni�cie nast�pi�o na linii
        bool isOnLine = IsPointOnLine(localClickPosition);

        if (isOnLine)
        {

            Debug.Log("Mouse has exited the line renderer");
        }
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
