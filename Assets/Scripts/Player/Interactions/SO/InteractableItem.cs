using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InteractionItem", menuName = "ScriptableObjects/InteractionItem", order = 1)]
public class InteractableItem : ScriptableObject
{
     public string _name;
     public Sprite _sprite;
     public string _description;
     public List<TPThoughtSO> UnlockableThoughts = new();
    // Przyda�oby si� �eby przy ka�dym itemie da�o si� zmieni� exposed propery �eby odblowkowa�y sui� nowe dialogi
    //[SerializeField] private List<bool> ExposedProperties;
}
