using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ItemWithDialogueInteraction : SampleInteraction
{
    [SerializeField] private InteractableItem _item;
    private void OnEnable()
    {
       // _prompt = _item.name;
    }
    public override bool Interact(Interactor interactor)
    {
        base.Interact(interactor);
        //PlayerController.Instance.SetDialogue(_dialogue);
        //Tu trzeba zrobi� referencje do UI z itemem i podmienia� opis i obrazek 
        return true;
    }
}
