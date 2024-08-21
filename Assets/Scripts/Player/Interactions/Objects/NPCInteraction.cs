using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : SampleInteraction
{
    [SerializeField] private DSDialogue _dialogue;

    public override bool Interact(Interactor interactor)
    {
        base.Interact(interactor);
        PlayerController.Instance.SetDialogue(_dialogue);

        return true;
    }
}
