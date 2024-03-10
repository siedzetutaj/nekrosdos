using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCoursor : MonoBehaviourSingleton<SetCoursor>
{
    public Texture2D lens;
    public Texture2D arrow;
    public Texture2D talk;
    public Vector2 MouseOffset;
    void Start()
    {
        SetCurosr(CoursorType.arrow); 
    }
    public void SetCurosr(CoursorType type)
    {
        switch (type)
        {
            case CoursorType.lens:
                {
                    SetTexture(lens);
                    break;
                }
            case CoursorType.talk:
                {
                    SetTexture(talk);
                    break;
                }
            default:
                {
                    SetTexture(arrow);
                    break;
                }
        }
    }
    private void SetTexture(Texture2D tex)
    {
        CursorMode mode = CursorMode.ForceSoftware;
        var xspot = tex.width / 2;
        var yspot = tex.height / 2;
        Vector2 hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(tex, hotSpot, mode);
    }
}

public enum CoursorType
{
    lens,
    talk,
    arrow
}