using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCoursor : MonoBehaviourSingleton<SetCoursor>
{
    public Texture2D move;
    public Texture2D scope;
    public Texture2D arrow;
    public Texture2D talk;

    void Start()
    {
        SetDefaultTexture();
    }
    public void SetTexture(Texture2D newTexture)
    {
        Cursor.SetCursor(newTexture,Vector2.zero, CursorMode.ForceSoftware);
    }
    public void SetDefaultTexture()
    {
        Cursor.SetCursor(arrow, Vector2.zero, CursorMode.ForceSoftware);
    }
}
