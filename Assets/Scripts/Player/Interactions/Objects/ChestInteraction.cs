using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private CoursorType _coursorType;
    public string InteractionPrompt => _prompt;

    public CoursorType CursorType => _coursorType;

    public bool Interact(Interactor interactor)
    {
        PlayerMovement.Instance.MoveToInteractable(transform.position);
        return true;
    }
}
