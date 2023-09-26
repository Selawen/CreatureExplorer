using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FallingState : State
{
    [SerializeField] private float painfulVelocity = 3f;
    [SerializeField] private float cripplingVelocity = 5f;
    [SerializeField] private float lethalVelocity = 10f;

    [SerializeField] private float aerialSpeed = 4f;
    [SerializeField] private float maxHorizontalVelocity = 5f;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private UnityEvent onLethalLanding;

    private new Rigidbody rigidbody;

    private Vector2 moveInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStateUpdate()
    {
        if(Physics.CheckSphere(transform.position, 0.25f, ~playerLayer))
        {
            if(rigidbody.velocity.y <= -lethalVelocity)
            {
                // Die
                onLethalLanding?.Invoke();
                return;
            }
            if(rigidbody.velocity.y <= -cripplingVelocity)
            {
                // Incapacitate the player for a while
                Owner.SwitchState(typeof(CrippledState));
                return;
            }
            if(rigidbody.velocity.y <= -painfulVelocity)
            {
                // This hurts the player a bit for a short while.
                Owner.SwitchState(typeof(HurtState));
                return;
            }
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

            Vector3 newVelocity = moveDirection.normalized * aerialSpeed;

            rigidbody.AddForce(newVelocity, ForceMode.Acceleration);

            Vector2 horizontalVelocity = new(rigidbody.velocity.x, rigidbody.velocity.z);

            if(horizontalVelocity.sqrMagnitude >= maxHorizontalVelocity * maxHorizontalVelocity)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxHorizontalVelocity;
                rigidbody.velocity = new(horizontalVelocity.x, rigidbody.velocity.y, horizontalVelocity.y);
            }

        }
    }
}
