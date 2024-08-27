using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : SampleInteraction
{
    [SerializeField] private DSDialogue _dialogue;

    public override void Interaction()
    {
        PlayerController.Instance.DisplayDialogue(_dialogue);
    }
}
