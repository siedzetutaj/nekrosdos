using System;
using UnityEngine;


public class TPThoughtSO : ScriptableObject
{
    [field: SerializeField] public SerializableGuid ID { get; private set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField][field: TextArea(10, 20)] public string Description { get; set; }
    [field: SerializeField] public Vector2 Postion { get; set; }

    public void SetID()
    {
        ID = Guid.NewGuid();
        Debug.Log(ID);
    }
}
