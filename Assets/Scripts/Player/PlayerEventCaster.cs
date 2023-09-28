using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventCaster : MonoBehaviour
{
    private UnityEvent<Vector3, float> makeSound;

    private void Start()
    {
        makeSound = new UnityEvent<Vector3, float>();

        foreach (Creature c in GameObject.FindObjectsOfType<Creature>())
        {
            makeSound.AddListener(c.ReactToPlayer);
        }
    }

    private void FixedUpdate()
    {
        makeSound.Invoke(transform.position, 5);
    }
}
