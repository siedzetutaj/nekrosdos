using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWithDialogueInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private CoursorType _coursorType;
    public string InteractionName => _prompt;

    public CoursorType CursorType => _coursorType;

    public bool Interact(Interactor interactor)
    {
        PlayerController.Instance.MoveToInteractable(transform.position);
        return true;
    }
}
