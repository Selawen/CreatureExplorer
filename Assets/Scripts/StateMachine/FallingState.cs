using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingState : State
{
    [SerializeField] private float painfulVelocity = 3f;
    [SerializeField] private float cripplingVelocity = 5f;
    [SerializeField] private float lethalVelocity = 10f;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Camera firstPersonCamera;

    private float defaultEyeHeight;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        defaultEyeHeight = firstPersonCamera.transform.position.y - transform.position.y;
    }

    public override void OnStateUpdate()
    {
        if(Physics.CheckSphere(transform.position, 0.25f, groundLayer))
        {
            if(rigidbody.velocity.y <= -lethalVelocity)
            {
                // Die
                return;
            }
            if(rigidbody.velocity.y <= -cripplingVelocity)
            {
                // Incapacitate the player for a while
                return;
            }
            if(rigidbody.velocity.y <= -painfulVelocity)
            {
                // This hurts the player a bit for a short while.
            }
            Owner.SwitchState(typeof(WalkingState));
        }

        firstPersonCamera.transform.position = new(transform.position.x, transform.position.y + defaultEyeHeight, transform.position.z);

    }
}
