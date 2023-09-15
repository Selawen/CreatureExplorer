using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class WalkingState : State
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;

    [SerializeField] private Camera firstPersonCamera;

    [SerializeField] private LayerMask groundLayer;

    private float eyeHeight;
    private float standardColliderHeight;

    private bool isSprinting;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        standardColliderHeight = capsuleCollider.height;

        eyeHeight = firstPersonCamera.transform.position.y - transform.position.y;
    }

    public override void OnStateEnter()
    {
        capsuleCollider.height = 2f;
        capsuleCollider.height = standardColliderHeight;
        capsuleCollider.center = Vector3.up * (standardColliderHeight * 0.5f);
    }

    public override void OnStateFixedUpdate()
    {
        if(!Physics.CheckSphere(transform.position, 0.25f, groundLayer))
        {
            Owner.SwitchState(typeof(FallingState));
            return;
        }
        Move();
        firstPersonCamera.transform.position = new(transform.position.x, transform.position.y + eyeHeight, transform.position.z);
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
        if (callbackContext.started && Physics.CheckSphere(transform.position, 0.25f, groundLayer))
        {
            Owner.SwitchState(typeof(JumpingState));
        }
    }

    private void Move()
    {
        if (moveInput.sqrMagnitude >= 0.1f)
        {
            float speed = isSprinting ? sprintSpeed : walkSpeed;
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + rigidbody.transform.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float verticalVelocity = rigidbody.velocity.y;

            Vector3 newVelocity = moveDirection.normalized * speed;

            newVelocity.y = verticalVelocity;

            rigidbody.velocity = newVelocity;
        }
    }

}
