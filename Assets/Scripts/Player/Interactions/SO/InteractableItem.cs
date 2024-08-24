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
    // Przyda³oby siê ¿eby przy ka¿dym itemie da³o siê zmieniæ exposed propery ¿eby odblowkowa³y suiê nowe dialogi
    //[SerializeField] private List<bool> ExposedProperties;
}
