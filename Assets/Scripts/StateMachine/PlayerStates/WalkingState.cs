using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class WalkingState : State
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float strafeSprintSpeed = 8f;
    [SerializeField] private float maxSprintAngle = 15f;

    [SerializeField] private LayerMask playerLayer;

    private bool isSprinting;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        if(strafeSprintSpeed >= sprintSpeed)
        {
            throw new System.Exception("Strafe Sprint Speed can't be higher than or as fast as sprint speed! Strafing must be slower than forward sprinting!");
        }
    }

    public override void OnStateFixedUpdate()
    {
        if(!Physics.CheckSphere(transform.position, 0.25f, ~playerLayer))
        {
            Owner.SwitchState(typeof(FallingState));
            return;
        }
        Move();
    }

    public void GetSprintInput(InputAction.CallbackContext callbackContext)
    {
        isSprinting = callbackContext.performed;
    }

    public void GetMoveInput(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>().normalized;
    }

    public void GetJumpInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started && Physics.CheckSphere(transform.position, 0.25f, ~playerLayer) && Owner.CurrentState.GetType() == GetType())
        {
            Owner.SwitchState(typeof(JumpingState));
        }
    }

    private void Move()
    {
        if (moveInput.sqrMagnitude >= 0.1f)
        {
            float speed = walkSpeed;
            float inputAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            float targetAngle = inputAngle + rigidbody.transform.eulerAngles.y;

            if (isSprinting)
            {
                speed = Mathf.Abs(inputAngle) <= maxSprintAngle ? sprintSpeed : strafeSprintSpeed;
            }

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float verticalVelocity = rigidbody.velocity.y;

            Vector3 newVelocity = moveDirection.normalized * speed;

            newVelocity.y = verticalVelocity;

            rigidbody.velocity = newVelocity;
        }
    }
}
