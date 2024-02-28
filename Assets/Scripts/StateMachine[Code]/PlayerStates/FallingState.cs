using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(Rigidbody))]
public class FallingState : State
{
    [SerializeField] private float painfulVelocity = 3f;
    [SerializeField] private float cripplingVelocity = 5f;
    [SerializeField] private float lethalVelocity = 10f;

    [SerializeField] private float aerialSpeed = 4f;
    [SerializeField] private float maxHorizontalVelocity = 5f;

    [SerializeField] private AudioSource dbg_SharedSource;
    [SerializeField] private AudioClip dbg_DeathSound;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private UnityEvent onLethalLanding;

    //private float fallVelocity;
    [SerializeField] private Rigidbody rigidbody;

    private Vector2 moveInput;

    private void Awake()
    {
        //rigidbody = GetComponent<Rigidbody>();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject != gameObject)
    //    {
    //        Debug.Log(rigidbody.velocity.y);
    //    }
    //}

    public override void OnStateUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.up * -1f, out RaycastHit hit, 1f, ~playerLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.TryGetComponent(out BounceSurface surface))
            {
                if (surface.Bounce(rigidbody.velocity.y * -1, out float exitForce))
                {
                    rigidbody.velocity = new(rigidbody.velocity.x, 0, rigidbody.velocity.z);
                    rigidbody.AddForce(exitForce * Vector3.up, ForceMode.VelocityChange);
                    return;
                }
            }
        }
        if (Physics.CheckSphere(transform.position, 0.25f, ~playerLayer, QueryTriggerInteraction.Ignore))
        {
            if (rigidbody.velocity.y <= -lethalVelocity)
            {
                // Die
                dbg_SharedSource.clip = dbg_DeathSound;
                dbg_SharedSource.Play();
                onLethalLanding?.Invoke();
                return;
            }
            if (rigidbody.velocity.y <= -cripplingVelocity)
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

    //public override void OnStateExit()
    //{
    //    fallVelocity = 0;
    //}

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
