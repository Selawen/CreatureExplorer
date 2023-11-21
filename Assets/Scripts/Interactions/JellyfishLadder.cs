using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishLadder : MonoBehaviour, IClimbable, IInteractable
{
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Climb [E]";
    public Vector3 ContactPoint { get; set; }

    public void Interact()
    {
    }
}
