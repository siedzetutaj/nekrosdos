using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class LineController : MonoBehaviour
{
    public bool IsDraggedByMouse = true;
    
    [SerializeField] private UILineRenderer lineRenderer;

    private InputSystem inputSystem;
    private void Start()
    {
        inputSystem = FindObjectOfType<InputSystem>();
    }
    private void Update()
    {
        SetEndPointToMousePosition();
    }
    private void SetEndPointToMousePosition()
    {
        if (IsDraggedByMouse)
        {
            //calculate mouse position
            Vector2 mousePosCenter = inputSystem.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
            mousePosCenter.x -= Screen.width / 2f;
            mousePosCenter.y -= Screen.height / 2f;
            SetPointPosition(1, mousePosCenter);
        }
    }
    public void SetPointPosition(int point, Vector2 pos)
    {
        lineRenderer.Points[point] = pos;
        lineRenderer.SetAllDirty();
    }
}
