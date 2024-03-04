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
        SetTexture(CoursorType.arrow);
    }
    public void SetTexture(CoursorType type)
    {
        switch (type)
        {
            case CoursorType.move:
                {
                    Cursor.SetCursor(move, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                }
            case CoursorType.scope:
                {
                    Cursor.SetCursor(scope, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                }
            case CoursorType.talk:
                {
                    Cursor.SetCursor(talk, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                }
            default:
                {
                    Cursor.SetCursor(arrow, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                }
        }
    }

}
public enum CoursorType
{
    move,
    scope,
    talk,
    arrow
}