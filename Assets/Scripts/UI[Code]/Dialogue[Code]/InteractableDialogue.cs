using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDialogue : MonoBehaviour, IInteractable, IDialogue
{
    [SerializeField] private string interactionPrompt;
    [SerializeField] private string[] dialogueText;

    [SerializeField] private bool showOnce;

    [ShowOnly] private bool hasBeenShown;

    public string InteractionPrompt => interactionPrompt;
    public string[] DialogueText => dialogueText;

    public void Interact()
    {
        if (!showOnce || (showOnce && !hasBeenShown))
        {
            DialogueUI.ShowText(dialogueText);
            hasBeenShown = true;
        }
    }
}
