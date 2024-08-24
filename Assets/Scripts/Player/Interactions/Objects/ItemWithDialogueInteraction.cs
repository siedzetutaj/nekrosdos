using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ItemWithDialogueInteraction : SampleInteraction
{
    [SerializeField] private InteractableItem _item;
    private void OnEnable()
    {
        _prompt = _item.name;
    }
    public override void Interaction()
    {
        foreach(TPThoughtSO thought in _item.UnlockableThoughts)
        {
            UIInformationDisplay.Instance.UnlockThoughtInTPPanel(thought);
        }
    }
}
