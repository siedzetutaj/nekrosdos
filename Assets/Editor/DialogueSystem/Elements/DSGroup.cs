using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements
{
    public class DSGroup : Group
    {
        public string ID { get; set; }
        public string OldTitle { get; set; }
        public bool WasModified { get; set; } = true;

        private Color defaultBorderColor;
        private float defaultBorderWidth;
        public DSGroup(string groupTitle, Vector2 position)
        {

            ID = Guid.NewGuid().ToString();

            title = groupTitle;
            OldTitle = groupTitle;

            SetPosition(new Rect(position, Vector2.zero));

            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle()
        {
            WasModified = true;
            contentContainer.style.borderBottomColor = defaultBorderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
        public override void OnSelected()
        {
            base.OnSelected();  // Call the base method to preserve default behavior
            WasModified = true; // Mark the node as modified when it is selected
        }
    }
}