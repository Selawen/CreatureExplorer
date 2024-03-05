using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingState : State
{
    [SerializeField] private float climbingSpeed = 3f;
    [SerializeField] private LayerMask playerLayer;

    private Vector2 moveInput;
    [SerializeField] private Rigidbody rb;

    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
    }
    public override void OnStateEnter()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
    }
    public override void OnStateFixedUpdate()
    {
        if(Physics.Raycast(transform.position, Vector3.up * -1, 0.5f, ~playerLayer) && moveInput.y < 0)
        {
            Owner.SwitchState(typeof(FallingState));
            return;
        }
        if(!Physics.Raycast(transform.position, transform.forward, 1f, ~playerLayer) && !Physics.Raycast(transform.position + Vector3.up, transform.forward, 1f, ~playerLayer))
        {
            rb.AddForce(transform.forward * 5f, ForceMode.VelocityChange);
            Owner.SwitchState(typeof(FallingState));
            return;
        }

        Climb();
    }

    public override void OnStateExit()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;
    }
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;
    }

    private void Climb()
    {
        rb.MovePosition(transform.position + climbingSpeed * Time.fixedDeltaTime * moveInput.y * Vector3.up);
    }
}
