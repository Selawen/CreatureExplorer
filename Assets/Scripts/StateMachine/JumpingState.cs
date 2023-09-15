using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpingState : State
{
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float aerialSpeed;
    //[SerializeField] private float standingEyeHeight = 1.7f;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Camera firstPersonCamera;

    private float defaultEyeHeight;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        defaultEyeHeight = firstPersonCamera.transform.position.y - transform.position.y;
    }
    public override void OnStateEnter()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public override void OnStateUpdate()
    {
        if(!Physics.CheckSphere(transform.position, 0.25f, groundLayer) && rigidbody.velocity.y <= 0)
        {
            Owner.SwitchState(typeof(FallingState));
        }
    }
    public override void OnStateFixedUpdate()
    {
        Move();
        firstPersonCamera.transform.position = new(transform.position.x, transform.position.y + defaultEyeHeight, transform.position.z);
    }
    
    private void Move()
    {
        if (moveInput.sqrMagnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + rigidbody.transform.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float verticalVelocity = rigidbody.velocity.y;

            Vector3 newVelocity = moveDirection.normalized * aerialSpeed;

            newVelocity.y = verticalVelocity;

            rigidbody.velocity = newVelocity;
        }
    }
}
