using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationInteraction : SampleInteraction
{
    public override bool Interact(Interactor interactor)
    {
        base.Interact(interactor);
        //Load next scene
        return true;
    }
}
