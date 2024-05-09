using UnityEngine;
using UnityEngine.UI;

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
        if(IsDraggedByMouse)
        {
            Vector2 mousePosCenter = inputSystem.mouseInput.TPInputs.MousePosition.ReadValue<Vector2>();
            mousePosCenter.x -= Screen.width / 2f;
            mousePosCenter.y -= Screen.height / 2f;
            SetPosioton(1, mousePosCenter);

        }
    }
    public void SetPosioton(int point, Vector2 pos)
    {
        lineRenderer.Points[point] = pos;
    }
}
