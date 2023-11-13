using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishLadder : MonoBehaviour, IInteractable, IClimbable
{
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Climb";
    public Vector3 ContactPoint { get; set; }

    public void Interact()
    {

    }
}
