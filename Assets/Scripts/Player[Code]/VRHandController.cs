using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class VRHandController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onLookAtPalm;
    [SerializeField] private UnityEvent onLookFromPalm;

    private bool lookingAtPalm = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckHandOrientation();
    }

    void CheckHandOrientation()
    {
        if (!lookingAtPalm )
        {
            if (transform.rotation.y > 160 && transform.rotation.y < 200)
            {
                lookingAtPalm = true;
                onLookAtPalm?.Invoke();
            }
        } else if (transform.rotation.y > 160 && transform.rotation.y < 200)
        {
            lookingAtPalm = false;
            onLookFromPalm?.Invoke();
        }
    }
}
