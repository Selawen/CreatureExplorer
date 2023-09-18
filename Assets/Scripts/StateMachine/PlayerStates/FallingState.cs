using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class FallingState : State
{
    [SerializeField] private float painfulVelocity = 3f;
    [SerializeField] private float cripplingVelocity = 5f;
    [SerializeField] private float lethalVelocity = 10f;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private UnityEvent onLethalLanding;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStateUpdate()
    {
        if(Physics.CheckSphere(transform.position, 0.25f, groundLayer))
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
}
