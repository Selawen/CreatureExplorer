using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Rigidbody))]
public class HurtState : State
{
    [SerializeField] private float hurtTime = 5f;
    [SerializeField] private float hurtMoveSpeed = 3.5f;
    [SerializeField] private float vignetteStrength = 0.3f;

    [SerializeField] private AudioSource sharedPlayerSource;
    [SerializeField] private AudioClip hurtSound;

    [SerializeField] private Volume volume;

    private float timer = 0f;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        sharedPlayerSource.clip = hurtSound;
        sharedPlayerSource.Play();
        if(volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = vignetteStrength;
        }
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        timer += Time.deltaTime;
        if (timer >= hurtTime)
        {
            timer = 0f;
            Owner.SwitchState(typeof(WalkingState));
        }

        Move();
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = 0;
        }
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

            Vector3 newVelocity = moveDirection.normalized * hurtMoveSpeed;

            newVelocity.y = verticalVelocity;

            rigidbody.velocity = newVelocity;

            return;
        }
        rigidbody.velocity = rigidbody.velocity.y * Vector3.up;
    }
}
