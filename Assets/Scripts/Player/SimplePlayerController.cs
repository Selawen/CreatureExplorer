using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerController : MonoBehaviour
{

    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sneakSpeed;

    [SerializeField] private float eyeHeight = 1.7f;

    [SerializeField] private float mouseSensitivity = 0.1f;

    private float basePlayerHeight;

    private bool isSprinting;
    private bool isCrouching;

    private new Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private Camera firstPersonCamera;

    private Vector2 moveInput;

    private void Awake()
    {
        firstPersonCamera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        basePlayerHeight = capsuleCollider.height;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }
    private void LateUpdate()
    {
        float currentEyeHeight = isCrouching ? 0.5f * eyeHeight : eyeHeight;
        firstPersonCamera.transform.position = new Vector3(transform.position.x, currentEyeHeight, transform.position.z);
    }
    public void ReceiveCrouchInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            ToggleCrouch();
        }
    }
    public void ReceiveSprintInput(InputAction.CallbackContext callbackContext)
    {
        isSprinting = callbackContext.performed;

        if (isSprinting && isCrouching)
            isCrouching = false;

    } 
    public void ReceiveRotationInput(InputAction.CallbackContext callbackContext)
    {
        HandleRotation(callbackContext.ReadValue<Vector2>().x);
    }
    public void ReceiveMoveInput(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>().normalized;
    }
    private void HandleRotation(float horizontalDelta)
    {
        transform.Rotate(new Vector3(0, horizontalDelta * mouseSensitivity, 0));
        firstPersonCamera.transform.rotation = transform.rotation;
    }
    private void HandleMovement()
    {
        if (moveInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float speed = walkSpeed;
            if(isSprinting || isCrouching)
            {
                speed = isSprinting ? sprintSpeed : sneakSpeed;
            }
            rigidbody.MovePosition(transform.position + (speed * Time.fixedDeltaTime * moveDirection.normalized));
        }
    }
    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        capsuleCollider.height = isCrouching? 0.5f * basePlayerHeight : basePlayerHeight;
        capsuleCollider.center = Vector3.up * (0.5f * capsuleCollider.height);
    }
}
