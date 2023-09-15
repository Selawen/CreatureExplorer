using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CrouchingState : State
{
    [SerializeField] private float sneakSpeed = 3f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchEyeHeight = 0.8f;

    [SerializeField] private Camera firstPersonCamera;

    private float defaultEyeHeight;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private CapsuleCollider capsuleCollider;

    private bool crouching;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public override void OnStateEnter()
    {
        capsuleCollider.height = crouchHeight;
        capsuleCollider.center = Vector3.up * (crouchHeight * 0.5f);

        firstPersonCamera.transform.position = new(transform.position.x, transform.position.y + crouchEyeHeight, transform.position.z);
    }

    public override void OnStateUpdate()
    {
        firstPersonCamera.transform.position = new(transform.position.x, firstPersonCamera.transform.position.y, transform.position.z);
    }

    public override void OnStateFixedUpdate()
    {
        if (moveInput.sqrMagnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + rigidbody.transform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float verticalVelocity = rigidbody.velocity.y;

            Vector3 newVelocity = moveDirection.normalized * sneakSpeed;

            newVelocity.y = verticalVelocity;

            rigidbody.velocity = newVelocity;
        }
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        firstPersonCamera.transform.position = new(transform.position.x, transform.position.y + defaultEyeHeight, transform.position.z);
    }

    public void GetMoveInput(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>().normalized;
    }

    public void GetCrouchInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (crouching)
            {
                crouching = false;
                Owner.SwitchState(typeof(WalkingState));
                return;
            }
            crouching = true;
            Owner.SwitchState(GetType());
        }
    }
}
