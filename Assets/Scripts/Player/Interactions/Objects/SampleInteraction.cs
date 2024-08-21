using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private CoursorType _coursorType;
    [SerializeField] private Transform _moveToThisTransform;
    public string InteractionName => _prompt;

    public CoursorType CursorType => _coursorType;

    public virtual bool Interact(Interactor interactor)
    {
        if (_moveToThisTransform == null)
            _moveToThisTransform = transform;

        PlayerController.Instance.MoveToInteractable(_moveToThisTransform.position);
        return true;
    }
}
