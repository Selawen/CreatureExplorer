using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CC_PlayerController : MonoBehaviour
{
    public float Loudness { get; private set; }

    [Header("Physical Stats")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sneakSpeed = 3f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float airSpeed = 4f;
    [SerializeField] private float strafeSprintSpeed = 7f;
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private float walkingLoudness = 1f;
    [SerializeField] private float sneakingLoudness = 0.5f;
    [SerializeField] private float sprintingLoudness = 2.5f;

    [Header("Settings")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float maxSprintAngle = 15f;
    [SerializeField] private float maxViewAngle = 70f;

    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float interactRadius = 5f;
    [SerializeField] private float interactHeight = 0.875f;

    [SerializeField] private float minimumClimbDistance = 1f;

    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private UnityEvent<string> onInteractPromptChanged;

    [SerializeField] private GameSettings gameSettings;

    private float defaultPlayerHeight;
    private float defaultCameraHeight;
    private float crouchEyeOffset;

    private FollowTarget cameraFollow;
    private PlayerInput playerInput;
    private CharacterController controller;
    private Vector2 moveInput;

    private Vector3 moveDirection;
    private float verticalRotation;
    private float verticalSpeed;

    private bool sprinting;
    private bool crouching;

    private IInteractable closestInteractable;

    private enum CharacterState { Grounded, Aerial, Climbing, Awaiting }
    private CharacterState currentState;

    private void Awake()
    {
        if (firstPersonCamera == null)
        {
            firstPersonCamera = Camera.main;
        }
        verticalRotation = firstPersonCamera.transform.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        defaultPlayerHeight = controller.height;
        defaultCameraHeight = defaultPlayerHeight;
        crouchEyeOffset = defaultPlayerHeight - crouchHeight;
        cameraFollow = firstPersonCamera.GetComponent<FollowTarget>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Scrapbook.OnBeginType += StartTyping;
        Scrapbook.OnEndType += StopTyping;

        cameraFollow.ChangeOffset(Vector3.up * defaultCameraHeight);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CharacterState.Grounded:
                Move();
                HandleInteract();
                break;
            case CharacterState.Aerial:
                Fall();
                break;
            case CharacterState.Climbing:
                Climb();
                break;
        }
        controller.Move((moveDirection + Vector3.up * verticalSpeed) * Time.deltaTime);
    }

    public void GetMoveInput(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    public void GetRotationInput(InputAction.CallbackContext context)
    {
        if (currentState == CharacterState.Awaiting) 
            return;

        Vector2 lookInput = context.ReadValue<Vector2>();
        verticalRotation = Mathf.Clamp(verticalRotation - (lookInput.y * gameSettings.LookSensitivity), -maxViewAngle, maxViewAngle);
        if(currentState != CharacterState.Climbing)
        {
            transform.Rotate(new Vector3(0, lookInput.x * gameSettings.LookSensitivity, 0));
        }

        firstPersonCamera.transform.rotation = Quaternion.Euler(new Vector3(verticalRotation, transform.eulerAngles.y, 0));
    }

    public void GetSprintInput(InputAction.CallbackContext context)
    {
        sprinting = context.performed;
        Loudness = sprinting ? sprintingLoudness : walkingLoudness;
        if(sprinting && crouching)
        {
            crouching = false;
            controller.height = defaultPlayerHeight;
            controller.center = controller.height * 0.5f * Vector3.up;
            cameraFollow.ChangeOffset(Vector3.up * defaultCameraHeight);
        }

    }

    public void GetCrouchInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            crouching = !crouching;
            Loudness = crouching ? sneakingLoudness : walkingLoudness;
            controller.height = crouching ? crouchHeight : defaultPlayerHeight;
            controller.center = controller.height * 0.5f * Vector3.up;
            cameraFollow.ChangeOffset(crouching ? Vector3.up * (defaultCameraHeight - crouchEyeOffset) : Vector3.up * defaultCameraHeight);
        
        }
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

    public void GetInteractInput(InputAction.CallbackContext context)
    {
        if (context.started && closestInteractable != null)
        {
            if(closestInteractable.GetType() == typeof(JellyfishLadder))
            {
                StartCoroutine(PrepareClimb(closestInteractable as JellyfishLadder));
                return;
            }
            closestInteractable.Interact();
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
            if (crouching)
            {
                speed = sneakSpeed;
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
            if (!Physics.Raycast(transform.position + Vector3.up * interactDistance, transform.forward, minimumClimbDistance + 0.5f, ~playerLayer) && 
                !Physics.Raycast(transform.position, transform.forward, minimumClimbDistance + 0.5f, ~playerLayer))
            {
                moveDirection = transform.forward * walkSpeed;
                currentState = CharacterState.Aerial;
                return;
            }
            moveDirection = transform.up * moveInput.y * climbSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
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
    private void HandleInteract()
    {
        closestInteractable = null;

        Collider[] collisions = Physics.OverlapSphere(transform.position + transform.forward * interactDistance + Vector3.up * interactHeight, interactRadius, ~playerLayer);
        if(collisions.Length > 0)
        {
            Collider closest = null;
            foreach(Collider c in collisions)
            {
                Vector3 interactOrigin = transform.position + Vector3.up * interactHeight;
                if (Physics.Raycast(interactOrigin, c.transform.position - interactOrigin, out RaycastHit hit, interactDistance, ~playerLayer))
                {
                    if (hit.transform.gameObject != c.gameObject)
                    {
                        continue;
                    }
                }
                if(c.TryGetComponent(out IInteractable interactable))
                {
                    if(closest == null || Vector3.Distance(c.transform.position, transform.position) < Vector3.Distance(closest.transform.position, transform.position))
                    {
                        closest = c;
                        closestInteractable = interactable;
                        if(closestInteractable.GetType() == typeof(JellyfishLadder))
                        {
                            JellyfishLadder climbable = closestInteractable as JellyfishLadder;
                            Physics.Raycast(transform.position + Vector3.up * interactHeight, transform.forward, out RaycastHit contact, interactDistance * 2 , ~playerLayer);
                            climbable.ContactPoint = contact.point;
                        }
                    }
                }
            }
        }
        if(closestInteractable != null)
        {
            onInteractPromptChanged?.Invoke(closestInteractable.InteractionPrompt);
            return;
        }
        onInteractPromptChanged?.Invoke("");
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

    private IEnumerator PrepareClimb(JellyfishLadder ladder)
    {
        currentState = CharacterState.Awaiting;
        while(Vector3.Distance(transform.position + Vector3.up * interactHeight, ladder.ContactPoint) > minimumClimbDistance)
        {
            moveDirection = transform.forward * walkSpeed;
            yield return null;
        }
        verticalSpeed = 0;
        moveDirection = Vector3.zero;
        currentState = CharacterState.Climbing;
    }

    private void StartTyping()
    {
        playerInput.SwitchCurrentActionMap("Await"); 
        Debug.Log("Should swap to 'Await' action map");
    }

    private void StopTyping()
    {
        playerInput.SwitchCurrentActionMap("Scrapbook"); 
        Debug.Log("Should swap to 'Scrapbook' action map");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - transform.forward * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + transform.right * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - transform.right * groundCheckDistance, groundCheckRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * interactHeight + interactDistance * transform.forward, interactRadius);
    }
}
