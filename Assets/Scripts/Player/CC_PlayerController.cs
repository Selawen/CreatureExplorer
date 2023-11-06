using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_PlayerController : MonoBehaviour
{
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        controller.Move(transform.forward * Time.deltaTime);
    }
}
