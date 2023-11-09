using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CC_PlayerController : MonoBehaviour
{
    [Header("Physical Stats")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float airSpeed = 4f;
    [SerializeField] private float strafeSprintSpeed = 7f;

    //[SerializeField] private float mass = 68f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Settings")]
    [SerializeField] private float maxSprintAngle = 15f;
    [SerializeField] private float maxViewAngle = 70f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private GameSettings gameSettings;

    private CharacterController controller;
    private Vector2 moveInput;

    private Vector3 moveDirection;
    private float verticalRotation;
    private float verticalSpeed;

    private bool sprinting;
    private bool crouching;

    private enum CharacterState { Grounded, Aerial, Climbing }
    private CharacterState currentState;

    private void Awake()
    {
        if(firstPersonCamera == null)
        {
            firstPersonCamera = Camera.main;
        }
        verticalRotation = firstPersonCamera.transform.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CharacterState.Grounded:
                Move();
                break;
            case CharacterState.Aerial:
                Fall();
                break;
            case CharacterState.Climbing:
                Climb();
                break;
        }
        controller.Move((moveDirection + Vector3.up * verticalSpeed) * Time.deltaTime );
    }

    public void GetMoveInput(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    
    public void GetRotationInput(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        verticalRotation = Mathf.Clamp(verticalRotation - (lookInput.y * gameSettings.LookSensitivity), -maxViewAngle, maxViewAngle);
        transform.Rotate(new Vector3(0, lookInput.x * gameSettings.LookSensitivity, 0));

        firstPersonCamera.transform.rotation = Quaternion.Euler(new Vector3(verticalRotation, transform.eulerAngles.y, 0));
    }

    public void GetSprintInput(InputAction.CallbackContext context)
    {
        sprinting = context.performed;
        if(sprinting && crouching)
        {
            crouching = false;
        }
    }

    public void GetCrouchInput(InputAction.CallbackContext context)
    {
        crouching = context.performed;
        if(sprinting && crouching)
        {
            sprinting = false;
        }
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if(currentState == CharacterState.Grounded && context.started)
        {
            Jump();
        }
    }

    private void Move()
    {
        if (!GroundCheck())
        {
            currentState = CharacterState.Aerial;
            moveDirection = Vector3.zero;
            return;
        }
        if (moveInput.sqrMagnitude > 0.1f)
        {
            float speed = walkSpeed;
            float inputAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            float targetAngle = inputAngle + transform.eulerAngles.y;

            if (sprinting)
            {
                speed = Mathf.Abs(inputAngle) <= maxSprintAngle ? sprintSpeed : strafeSprintSpeed;
            }

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * speed;
        }
        else
        { 
            moveDirection = Vector3.zero;
        }
    }

    private void Climb()
    {
        if (moveInput.sqrMagnitude > 0.1f)
        {
            moveDirection = transform.up * climbSpeed * Time.deltaTime;
            //controller.Move(transform.up * climbSpeed * Time.deltaTime);
        }
    }

    private void Fall()
    {
        if (moveInput.sqrMagnitude > 0.1f)
        {
            float inputAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            float targetAngle = inputAngle + transform.eulerAngles.y;

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * airSpeed;
        }
        if (controller.velocity.y > 0)
        {
            verticalSpeed -= 9.81f * Time.deltaTime;
            return;
        }
        verticalSpeed -= 9.81f * Time.deltaTime;
        if (GroundCheck())
        {
            Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit, 2f, ~playerLayer);
            verticalSpeed = 0;

            transform.position = hit.point;
            currentState = CharacterState.Grounded;
            return;
        }
        //Vector3 fallingSpeed = new(controller.velocity.x, controller.velocity.y - (9.81f * Time.deltaTime), controller.velocity.z);

    }

    private void Jump()
    {
        verticalSpeed = jumpForce;
    }

    private bool GroundCheck()
    {
        // We're doing a front and back ground check for additional accuracy without making the radius too big.
        if (Physics.CheckSphere(transform.position + transform.forward * groundCheckDistance, groundCheckRadius, ~playerLayer))
        {
            return true;
        }
        if (Physics.CheckSphere(transform.position - transform.forward * groundCheckDistance, groundCheckRadius, ~playerLayer))
        {
            return true;
        }
        if (Physics.CheckSphere(transform.position + transform.right * groundCheckDistance, groundCheckRadius, ~playerLayer))
        {
            return true;
        }
        if (Physics.CheckSphere(transform.position - transform.right * groundCheckDistance, groundCheckRadius, ~playerLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - transform.forward * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + transform.right * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - transform.right * groundCheckDistance, groundCheckRadius);
    }
}
