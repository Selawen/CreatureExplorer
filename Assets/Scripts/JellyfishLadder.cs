using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishLadder : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Climb";

    public void Interact()
    {

    }
}
