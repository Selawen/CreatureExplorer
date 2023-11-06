using UnityEngine;

public class Monument : QuestableObject, IInteractable
{
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Interact";

    // Are we still going to be able to shift the Day/Night cycle through resting? This is setup if that's still something we want. - Justin
    //public delegate void OnMonumentInteractHandler();
    //public static OnMonumentInteractHandler OnMonumentInteract;

    public void Interact()
    {
        //OnMonumentInteract?.Invoke();
    }
}
