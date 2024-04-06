using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "123", menuName = "ScriptableObjects/123", order = 1)]
    public class TPSO : ScriptableObject
    {
        [field: SerializeField] public readonly Guid ID = Guid.NewGuid();
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Sprite Sprite { get; set; }
        [field: SerializeField][field: TextArea(10, 20)] public string Description { get; set; }
        [field: SerializeField] public Vector2 Postion { get; set; }

        public void Initialize(Sprite sprite, string text, Vector2 postion)
        {
            Sprite = sprite;
            Name = text;
            Postion = postion;
        }
    }
