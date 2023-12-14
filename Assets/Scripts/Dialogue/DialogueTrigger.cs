using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour, IDialogue
{
    [SerializeField] private string dialogueText;
    [SerializeField] private bool showOnce;
    [SerializeField] private bool hideOnExit;

    [ShowOnly] private bool hasBeenShown;

    public string DialogueText()
    {
        return dialogueText;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!showOnce || (showOnce && !hasBeenShown))
        {
            DialogueUI textCanvas = other.GetComponentInChildren<DialogueUI>();
            if (textCanvas != null)
            {
                textCanvas.ShowText(dialogueText);
                hasBeenShown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hideOnExit)
        {
            DialogueUI textCanvas = other.GetComponentInChildren<DialogueUI>();
            if (textCanvas != null)
            {
                textCanvas.HideText();
            }
        }
    }
}
