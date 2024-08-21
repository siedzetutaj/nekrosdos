using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _description;
}
