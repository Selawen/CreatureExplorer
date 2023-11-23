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
    [SerializeField] private float deadlyFallVelocity = 10f;

    [Header("Loudness")]
    [SerializeField] private float walkingLoudness = 1f;
    [SerializeField] private float sneakingLoudness = 0.5f;
    [SerializeField] private float sprintingLoudness = 2.5f;

    [Header("Settings")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float maxSprintAngle = 15f;
    [SerializeField] private float maxViewAngle = 70f;

    [Header("Interaction")]
    [SerializeField] private float climbDistance = 0.25f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float interactRadius = 5f;
    [SerializeField] private float interactHeight = 0.875f;
    [SerializeField] private UnityEvent<string> onInteractPromptChanged;

    [SerializeField] private float minimumClimbDistance = 1f;
    [SerializeField] private LayerMask playerLayer;

    [Header("GroundCheck")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Header("Death")]
    [SerializeField] private Transform respawnTransform;
    [SerializeField] private float respawnDuration = 0.5f;
    [SerializeField] private GameObject respawnOccluder;
    [SerializeField] private UnityEvent exitCamera;
    [SerializeField] private UnityEvent closeScrapbook;

    [SerializeField] private Camera firstPersonCamera;

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
    private float rotationSpeed = 1f;

    private float initialMomentumOnAirtimeStart;

    private bool sprinting;
    private bool crouching;
    private bool died = false;
    private MeshRenderer respawnFadeRenderer;

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

        cameraFollow = firstPersonCamera.GetComponent<FollowTarget>();

        defaultPlayerHeight = controller.height;
        
        crouchEyeOffset = defaultPlayerHeight - crouchHeight;

        playerInput = GetComponent<PlayerInput>();

        respawnFadeRenderer = Instantiate(respawnOccluder, firstPersonCamera.transform).GetComponent<MeshRenderer>();

        StaticQuestHandler.OnQuestOpened += () => 
        { 
            playerInput.SwitchCurrentActionMap("Scrapbook"); 
            onInteractPromptChanged?.Invoke(string.Empty);
            currentState = CharacterState.Awaiting;
        };
        StaticQuestHandler.OnQuestClosed += () =>
        {
            playerInput.SwitchCurrentActionMap("Overworld");
            currentState = CharacterState.Grounded;
        };

    }

    private void Start()
    {
        defaultCameraHeight = cameraFollow.TrueOffset.y;

        Scrapbook.OnBeginType += StartTyping;
        Scrapbook.OnEndType += StopTyping;
    }

    // Update is called once per frame
    void Update()
    {
        if (died)
            return;

        switch (currentState)
        {
            case CharacterState.Grounded:
                Move();
                HandleInteract();
                break;
            case CharacterState.Aerial:
                Fall();
                HandleInteract();
                break;
            case CharacterState.Climbing:
                Climb();
                break;
        }
        
        if (!died)
            controller.Move((moveDirection + Vector3.up * verticalSpeed) * Time.deltaTime);
    }

    public void SetRotationSpeed(float newSpeed) => rotationSpeed = newSpeed;

    public void GetMoveInput(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    public void GetRotationInput(InputAction.CallbackContext context)
    {
        if (currentState == CharacterState.Awaiting) 
            return;

        Vector2 lookInput = context.ReadValue<Vector2>();
        verticalRotation = Mathf.Clamp(verticalRotation - (lookInput.y * gameSettings.LookSensitivity * rotationSpeed), -maxViewAngle, maxViewAngle);
        if(currentState != CharacterState.Climbing)
        {
            transform.Rotate(new Vector3(0, lookInput.x * gameSettings.LookSensitivity * rotationSpeed, 0));
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
        if (currentState == CharacterState.Climbing || currentState == CharacterState.Awaiting) return;

        if (context.started && closestInteractable != null)
        {
            moveDirection = Vector3.zero;
            if(closestInteractable.GetType() == typeof(JellyfishLadder))
            {
                StartClimb();
                //StartCoroutine(PrepareClimb(closestInteractable as JellyfishLadder));
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
            initialMomentumOnAirtimeStart = controller.velocity.magnitude;
            //moveDirection = Vector3.zero;
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
            if(GroundCheck() && moveInput.y < 0)
            {
                currentState = CharacterState.Grounded;
            }
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

            Vector3 control = moveDirection + (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * airSpeed * 0.1f;
            if (control.magnitude > initialMomentumOnAirtimeStart)
            {
                control = control.normalized * initialMomentumOnAirtimeStart;
            }
            moveDirection = control;
        }

        verticalSpeed -= 9.81f * Time.deltaTime;
        if (GroundCheck())
        {
            if(Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit, 1f, ~playerLayer))
            {
                if(hit.transform.TryGetComponent(out BounceSurface surface))
                {
                    if(surface.Bounce(verticalSpeed * -1, out float exitForce))
                    {
                        verticalSpeed = exitForce;
                        return;
                    }
                }
            }
            if (verticalSpeed < -deadlyFallVelocity)
            {
                StartCoroutine(Die());
            }
            else
            {
                Physics.Raycast(transform.position, transform.up * -1, out RaycastHit floorHit, 2f, ~playerLayer);
                transform.position = floorHit.point;
                currentState = CharacterState.Grounded;
                return;
            }
        }
        //Vector3 fallingSpeed = new(controller.velocity.x, controller.velocity.y - (9.81f * Time.deltaTime), controller.velocity.z);

    }

    private void Jump()
    {
        verticalSpeed = jumpForce;
        initialMomentumOnAirtimeStart = controller.velocity.magnitude;
    }

    private void HandleInteract()
    {
        closestInteractable = null;

        if(Physics.Raycast(transform.position + Vector3.up * interactHeight, transform.forward, out RaycastHit climb, climbDistance, ~playerLayer))
        {
            if(climb.transform.TryGetComponent(out JellyfishLadder ladder))
            {
                ladder.ContactPoint = climb.point;
                closestInteractable = ladder;
                onInteractPromptChanged?.Invoke(closestInteractable.InteractionPrompt);
                return;
            }
        }
        Collider[] collisions = Physics.OverlapSphere(transform.position + transform.forward * interactDistance + Vector3.up * interactHeight, interactRadius, ~playerLayer);
        if (collisions.Length > 0)
        {
            Collider closest = null;
            foreach (Collider c in collisions)
            {
                // First, we check if the collisions we found can actually be seen from the player's perspective and aren't obscured by another object
                Vector3 interactOrigin = transform.position + Vector3.up * interactHeight;
                if (Physics.Raycast(interactOrigin, c.transform.position - interactOrigin, out RaycastHit hit, interactDistance, ~playerLayer))
                {
                    if (hit.transform.gameObject != c.gameObject)
                    {
                        continue;
                    }
                }
                if (c.TryGetComponent(out IInteractable interactable))
                {
                    if (interactable.GetType() == typeof(JellyfishLadder)) continue;

                    if (closest == null || Vector3.Distance(c.transform.position, transform.position) < Vector3.Distance(closest.transform.position, transform.position))
                    {
                        closest = c;
                        closestInteractable = interactable;
                        //if(closestInteractable.GetType() == typeof(JellyfishLadder))
                        //{
                        //    JellyfishLadder climbable = closestInteractable as JellyfishLadder;
                        //    Physics.Raycast(transform.position + Vector3.up * interactHeight, transform.forward, out RaycastHit contact, interactDistance * 2 , ~playerLayer);
                        //    climbable.ContactPoint = contact.point;
                        //}
                    }
                }
            }
        }

        if (closestInteractable != null)
        {
            onInteractPromptChanged?.Invoke(closestInteractable.InteractionPrompt);
            return;
        }
        onInteractPromptChanged?.Invoke(string.Empty);
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

    public void GoDie() => StartCoroutine(Die());

    private void StartClimb()
    {
        onInteractPromptChanged?.Invoke("");
        verticalSpeed = 0;
        moveDirection = Vector3.zero;
        currentState = CharacterState.Climbing;
    }

    //private IEnumerator PrepareClimb(JellyfishLadder ladder)
    //{
    //    currentState = CharacterState.Awaiting;
    //    onInteractPromptChanged?.Invoke("");
    //    while(Vector3.Distance(transform.position + Vector3.up * interactHeight, ladder.ContactPoint) > minimumClimbDistance)
    //    {
    //        moveDirection = transform.forward * walkSpeed;
    //        yield return null;
    //    }
    //    verticalSpeed = 0;
    //    moveDirection = Vector3.zero;
    //    currentState = CharacterState.Climbing;
    //}

    private IEnumerator Die()
    {
        died = true;
        moveDirection = Vector3.zero;
        verticalRotation = 0;
        verticalSpeed = 0;

        GameObject canvas = GetComponentInChildren<Canvas>().gameObject;
        canvas.SetActive(false);
        controller.enabled = false;
        float timer = 0.001f;

        Material fadeMaterial = respawnFadeRenderer.material;
        Color fadeColor = fadeMaterial.color;

        GetComponent<PlayerCamera>().DeleteCameraRoll();

        // Fade in vision obscurer, move player, then fade it out again
        while (timer < respawnDuration*0.3f)
        {
            fadeColor.a = Mathf.InverseLerp(0, 0.3f * respawnDuration, timer);
            fadeMaterial.color = fadeColor;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = respawnTransform.position;

        // TODO: refactor
        if (playerInput.currentActionMap.name == "Camera")
            exitCamera.Invoke();
        else if (playerInput.currentActionMap.name == "Scrapbook")
            closeScrapbook.Invoke();

        while (timer < respawnDuration)
        {
            fadeColor.a = Mathf.InverseLerp(respawnDuration, 0.6f* respawnDuration,timer);
            fadeMaterial.color = fadeColor;

            timer += Time.deltaTime;
            yield return null;
        }

        canvas.SetActive(true);
        controller.enabled = true;
        died = false;
    }

    public void StartTyping()
    {
        playerInput.actions.FindAction("QuickCloseBook").Disable();
    }

    public void StopTyping()
    {
        playerInput.actions.FindAction("QuickCloseBook").Enable();
    }

    private void OnDrawGizmos()
    {
        // Groundcheck
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - transform.forward * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + transform.right * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - transform.right * groundCheckDistance, groundCheckRadius);
        // Interaction
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * interactHeight + interactDistance * transform.forward, interactRadius);
        // Respawn
        Gizmos.color = Color.yellow;
        if (respawnTransform)
        {
            Gizmos.DrawSphere(respawnTransform.position, groundCheckRadius);
        }
    }
}
