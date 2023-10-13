using UnityEngine;

public class Monument : MonoBehaviour, IInteractable, IIdentifiable
{
    public string GivenName { get; set; } = "Monument";
    public string GivenDescription { get; set; } = "A monument to the Titan, a landmark you can fast travel to, under the right circumstances.";

    // Are we still going to be able to shift the Day/Night cycle through resting? This is setup if that's still something we want. - Justin
    //public delegate void OnMonumentInteractHandler();
    //public static OnMonumentInteractHandler OnMonumentInteract;

    public void Interact()
    {
        //OnMonumentInteract?.Invoke();
    }
}
