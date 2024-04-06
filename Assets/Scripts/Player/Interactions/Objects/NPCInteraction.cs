using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionName;
    [SerializeField] private CoursorType _coursorType;
    [SerializeField] private DSDialogue _dialogue;
    public string InteractionName => _interactionName;

    public CoursorType CursorType => _coursorType;

    public bool Interact(Interactor interactor)
    {
        PlayerController.Instance.MoveToInteractable(transform.position);
        PlayerController.Instance.SetDialogue(_dialogue);

        return true;
    }
}
