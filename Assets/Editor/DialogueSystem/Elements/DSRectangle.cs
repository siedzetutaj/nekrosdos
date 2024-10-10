//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class DSRectangle : GraphElement
//{
//    private VisualElement resizeHandle;
//    private bool isResizing;
//    private Vector2 initialSize;
//    private Vector2 initialMousePosition;
//    public DSRectangle(Vector2 position = default(Vector2))
//    {
//        // Set initial size
//        style.width = 100;
//        style.height = 100;

//        // Make the rectangle green
//        style.backgroundColor = Color.green;

//        // Add a resize handle (small box in the bottom-right corner)
//        resizeHandle = new VisualElement();
//        resizeHandle.style.width = 20;
//        resizeHandle.style.height = 20;
//        resizeHandle.style.backgroundColor = Color.black;
//        resizeHandle.style.position = Position.Absolute;
//        resizeHandle.style.bottom = 0;
//        resizeHandle.style.right = 0;

//        Add(resizeHandle);

//        // Mouse events for resizing
//        resizeHandle.RegisterCallback<MouseDownEvent>(OnMouseDown);
//        resizeHandle.RegisterCallback<MouseMoveEvent>(OnMouseMove);
//        resizeHandle.RegisterCallback<MouseUpEvent>(OnMouseUp);

//        // Enable dragging the element itself
            
//        this.AddManipulator(new Dragger());

//    }

//    private void OnMouseDown(MouseDownEvent evt)
//    {
//        isResizing = true;
//        initialSize = new Vector2(resolvedStyle.width, resolvedStyle.height);
//        initialMousePosition = evt.mousePosition;
//        evt.StopPropagation(); // Stop other handlers from receiving the event
//    }

//    private void OnMouseMove(MouseMoveEvent evt)
//    {
//        if (isResizing)
//        {
//            // Calculate new size based on mouse movement
//            Vector2 delta = evt.mousePosition - initialMousePosition;
//            style.width = Mathf.Max(50, initialSize.x + delta.x);  // Prevent rectangle from becoming too small
//            style.height = Mathf.Max(50, initialSize.y + delta.y);
//        }
//    }

//    private void OnMouseUp(MouseUpEvent evt)
//    {
//        isResizing = false;
//    }
//}