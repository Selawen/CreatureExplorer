using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class JumpingState : State
{
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float aerialSpeed;
    [SerializeField] private float maxHorizontalVelocity = 5f;

    [SerializeField] private LayerMask playerLayer;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStateEnter()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    public override void OnStateUpdate()
    {
        if(!Physics.CheckSphere(transform.position, 0.25f, ~playerLayer) && rigidbody.velocity.y <= 0)
        {
            Owner.SwitchState(typeof(FallingState));
            return;
        }
        if(Physics.CheckSphere(transform.position, 0.25f, ~playerLayer))
        {
            Owner.SwitchState(typeof(WalkingState));
        }
    }

    public override void OnStateFixedUpdate()
    {
        Move();
    }
    
    public void GetMoveInput(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>().normalized;
    }

    private void Move()
    {
        if (moveInput.sqrMagnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + rigidbody.transform.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float verticalVelocity = rigidbody.velocity.y;

            Vector3 newVelocity = moveDirection.normalized * aerialSpeed;

            rigidbody.AddForce(newVelocity, ForceMode.Acceleration);

            Vector2 horizontalVelocity = new(rigidbody.velocity.x, rigidbody.velocity.z);

            if (horizontalVelocity.sqrMagnitude >= maxHorizontalVelocity * maxHorizontalVelocity)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxHorizontalVelocity;
                rigidbody.velocity = new(horizontalVelocity.x, rigidbody.velocity.y, horizontalVelocity.y);
            }
        }
    }
}
