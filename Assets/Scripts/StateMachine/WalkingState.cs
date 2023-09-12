using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class WalkingState : State
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float standingEyeHeight = 1.7f;

    [SerializeField] private Camera firstPersonCamera;

    private float standardColliderHeight;
    private bool isSprinting;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;

    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        standardColliderHeight = capsuleCollider.height;
    }
    public override void OnStateEnter()
    {
        capsuleCollider.height = standardColliderHeight;
        capsuleCollider.center = Vector3.up * (standardColliderHeight * 0.5f);

        Vector3 camPosition = Camera.main.transform.position;
        Camera.main.transform.position = new(camPosition.x, standingEyeHeight, camPosition.z);
    }
    public override void OnStateFixedUpdate()
    {
        Move();

        firstPersonCamera.transform.position = new(transform.position.x, transform.position.y + standingEyeHeight, transform.position.z);
    }
    public void GetSprintInput(InputAction.CallbackContext callbackContext)
    {
        isSprinting = callbackContext.performed;
    }
    public void GetMoveInput(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>().normalized;
    }

    private void Move()
    {
        if (moveInput.magnitude >= 0.1f)
        {
            float speed = isSprinting ? sprintSpeed : walkSpeed;
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + rigidbody.transform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            rigidbody.MovePosition(rigidbody.transform.position + (speed * Time.fixedDeltaTime * moveDirection.normalized));
        }
    }

}
