using UnityEngine;

public interface IInteractable
{
    public string InteractionPrompt { get; }
    public void Interact();

}
