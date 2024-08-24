using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] protected string _prompt;
    [SerializeField] protected CoursorType _coursorType;
    [SerializeField] protected Transform _moveToThisTransform;
    public string InteractionName => _prompt;

    public CoursorType CursorType => _coursorType;

    public virtual bool Interact(Interactor interactor)
    {
        if (_moveToThisTransform == null)
            _moveToThisTransform = transform;

        PlayerController.Instance.SetInteraction(this, _moveToThisTransform.position);
        return true;
    }
    public virtual void Interaction()
    {
        //Override to give functionality
    } 
}
