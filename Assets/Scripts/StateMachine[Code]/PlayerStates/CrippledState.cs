using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//[RequireComponent(typeof(Rigidbody))]
public class CrippledState : State
{
    [SerializeField] private float crippleTime = 10f;
    [SerializeField] private float crippleMoveSpeed = 2f;
    [SerializeField] private float vignetteStrength = 0.5f;

    [SerializeField] private AudioSource sharedPlayerSource;
    [SerializeField] private AudioClip painSound;
    [SerializeField] private AudioClip boneCrackSound;

    [SerializeField] private Volume volume;

    private float timer = 0f;

    private Vector2 moveInput;

    [SerializeField] private Rigidbody rigidbody;
    private PhysicsStepper stepper;


    private void Awake()
    {
        //rigidbody = GetComponent<Rigidbody>();
        stepper = GetComponent<PhysicsStepper>();
    }

    public override void OnStateEnter()
    {
        sharedPlayerSource.clip = painSound;
        sharedPlayerSource.Play();
        sharedPlayerSource.clip = boneCrackSound;
        sharedPlayerSource.Play();
        if (volume == null)
        {
            return;
        }
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = vignetteStrength;
        }
    }

    public override void OnStateUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= crippleTime)
        {
            timer = 0f;
            Owner.SwitchState(typeof(WalkingState));
        }

        Move();
    }
    public override void OnStateExit()
    {
        if (volume == null)
        {
            return;
        }
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

            stepper.HandleStep(ref rigidbody, moveDirection);

            float verticalVelocity = rigidbody.velocity.y;

            Vector3 newVelocity = moveDirection.normalized * crippleMoveSpeed;

            newVelocity.y = verticalVelocity;

            rigidbody.velocity = newVelocity;

            return;
        }
        //rigidbody.velocity = rigidbody.velocity.y * Vector3.up;
    }
}
