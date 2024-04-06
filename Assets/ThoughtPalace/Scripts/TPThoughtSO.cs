using System;
using UnityEngine;


public class TPThoughtSO : ScriptableObject
{
    [field: SerializeField] public readonly Guid ID = Guid.NewGuid();
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField][field: TextArea(10, 20)] public string Description { get; set; }
    [field: SerializeField] public Vector2 Postion { get; set; }

}
