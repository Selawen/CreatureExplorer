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
    [SerializeField] private float hurtWalkingSpeed;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float painfulVelocity = 10f;
    [SerializeField] private float deadlyFallVelocity = 20f;

    [Header("Loudness")]
    [SerializeField] private float walkingLoudness = 1f;
    [SerializeField] private float sneakingLoudness = 0.5f;
    [SerializeField] private float sprintingLoudness = 2.5f;

    [Header("Settings")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float maxSprintAngle = 15f;
    [SerializeField] private float maxViewAngle = 70f;
    [SerializeField] private float hurtTime;

    [Header("Interaction")]
    [SerializeField] private float climbDistance = 0.25f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float interactRadius = 5f;
    [SerializeField] private float interactHeight = 0.875f;
    [SerializeField] private float drowningHeight = 1.6f;
    [SerializeField] private float throwForce = 4f;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private UnityEvent<string> onInteractPromptChanged;

    [SerializeField] private float minimumClimbDistance = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask waterLayer;


    [Header("Death")]
    [SerializeField] private Transform respawnTransform;
    [SerializeField] private float respawnDuration = 0.5f;
    [SerializeField] private GameObject deathScreen;
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
    private float hurtTimer;

    private float initialMomentumOnAirtimeStart;

    // This is serialized only for debugging purposes
    [SerializeField] private bool climbingUnlocked;
    private bool sprinting;
    private bool crouching;
    private bool isHurt;
    private bool died = false;
    private MeshRenderer respawnFadeRenderer;

    private IInteractable closestInteractable;
    private Throwable heldThrowable;

    private enum CharacterState { Grounded, Aerial, Climbing, Awaiting, Hurt }
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
        deathScreen = Instantiate(deathScreen);
        deathScreen.SetActive(false);

        GrandTemple.OnRingExtended += UnlockClimbing;

        StaticQuestHandler.OnQuestInputDisabled += () =>
        {
            playerInput.SwitchCurrentActionMap("Await");
            currentState = CharacterState.Awaiting;
        };

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

        if(Physics.CheckSphere(transform.position + Vector3.up * drowningHeight, 0.25f, waterLayer))
        {
            StartCoroutine(Die());
            return;
        }
        switch (currentState)
        {
            case CharacterState.Hurt:
                MoveHurt();
                HandleInteract();
                hurtTimer += Time.deltaTime; 
                if(hurtTimer >= hurtTime)
                {
                    currentState = CharacterState.Grounded;
                    isHurt = false;
                }
                break;
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
        {
            controller.Move((moveDirection + verticalSpeed * Vector3.up) * Time.deltaTime);
        }

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
        if(currentState == CharacterState.Grounded && context.performed)
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
                if (climbingUnlocked)
                {
                    StartClimb();
                    return;
                }
                // TODO: You can't climb, as the jellyfish shocks you.
                return;
            }
            if(closestInteractable.GetType() == typeof(Throwable) && heldThrowable == null)
            {
                heldThrowable = closestInteractable as Throwable;
                heldThrowable.transform.SetParent(throwPoint);
                heldThrowable.transform.localPosition = Vector3.zero;
            }
            closestInteractable.Interact();
        }
    }

    public void GetThrowInput(InputAction.CallbackContext context)
    {
        if(context.started && heldThrowable != null)
        {
            heldThrowable.Throw(firstPersonCamera.transform.forward, throwForce);
            heldThrowable = null;
        }
    }

    private void Move()
    {
        if (!controller.isGrounded)
        {
            currentState = CharacterState.Aerial;
            Vector2 horizontalVelocity = new Vector2(controller.velocity.x, controller.velocity.z);
            initialMomentumOnAirtimeStart = horizontalVelocity.magnitude;
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

    private void MoveHurt()
    {
        if (!controller.isGrounded)
        {
            currentState = CharacterState.Aerial;
            Vector2 horizontalVelocity = new Vector2(controller.velocity.x, controller.velocity.z);
            initialMomentumOnAirtimeStart = horizontalVelocity.magnitude;
            return;
        }
        if (moveInput.sqrMagnitude > 0.1f)
        {
            float inputAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            float targetAngle = inputAngle + transform.eulerAngles.y;

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * hurtWalkingSpeed;
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
            if(controller.isGrounded && moveInput.y < 0)
            {
                currentState = CharacterState.Grounded;
                return;
            }
            if (!Physics.Raycast(transform.position + Vector3.up * interactDistance, transform.forward, minimumClimbDistance + 0.5f, ~playerLayer) && 
                !Physics.Raycast(transform.position, transform.forward, minimumClimbDistance + 0.5f, ~playerLayer))
            {
                moveDirection = transform.forward * walkSpeed;
                currentState = CharacterState.Aerial;
                return;
            }
            moveDirection = moveInput.y * climbSpeed * transform.up;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void Fall()
    {
        float gravity = 9.81f * Time.deltaTime;
        if (controller.velocity.y > -0.5f)
            verticalSpeed = controller.velocity.y - gravity;
        else
            verticalSpeed -= gravity;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            float inputAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            float targetAngle = inputAngle + transform.eulerAngles.y;

            Vector3 control = moveDirection + (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * airSpeed * 0.1f;
            if (control.magnitude > initialMomentumOnAirtimeStart)
            {
                if(initialMomentumOnAirtimeStart < airSpeed)
                {
                    control = control.normalized * airSpeed;
                }
                else
                {
                    control = control.normalized * initialMomentumOnAirtimeStart;
                }
            }
            moveDirection = control;
        }
        else
        {
            // Slowly decay the speed if no input is given.
            // Below the threshold, we set the move direction to 0.
            if (moveDirection.magnitude > 0.2f)
            {
                moveDirection *= 0.95f;
            }
            else
            {
                moveDirection = Vector3.zero;
            }
        }

        if (controller.isGrounded)
        {
            currentState = CharacterState.Grounded;
            if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit, 1f, ~playerLayer))
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
            if (verticalSpeed < -painfulVelocity)
            {
                if (verticalSpeed < -deadlyFallVelocity)
                {
                    died = true;
                    StartCoroutine(Die());
                    return;
                }
                currentState = CharacterState.Hurt;
                hurtTimer = 0;
                isHurt = true;
                return;
            }
            verticalSpeed = -1f;
        }
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
            if(climb.transform.TryGetComponent(out JellyfishLadder ladder) && currentState != CharacterState.Hurt)
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

    public void GoDie() => StartCoroutine(Die());

    private void StartClimb()
    {
        onInteractPromptChanged?.Invoke("");
        verticalSpeed = 0;
        moveDirection = Vector3.zero;
        currentState = CharacterState.Climbing;
    }

    private IEnumerator Die()
    {
        died = true;
        moveDirection = Vector3.zero;
        verticalRotation = 0;
        verticalSpeed = -0.5f;

        GameObject canvas = GetComponentInChildren<Canvas>().gameObject;
        canvas.SetActive(false);
        controller.enabled = false;

        StartCoroutine(deathScreen.GetComponent<RandomMessage>().FadeIn(respawnDuration*0.1f));
        deathScreen.SetActive(true);

        GetComponent<PlayerCamera>().DeleteCameraRoll();

        Material fadeMaterial = respawnFadeRenderer.material;
        Color fadeColor = fadeMaterial.color;

        float timer = 0.001f;

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

        StartCoroutine(deathScreen.GetComponent<RandomMessage>().FadeOut(respawnDuration * 0.1f, respawnDuration *0.6f));

        while (timer < respawnDuration)
        {
            fadeColor.a = Mathf.InverseLerp(respawnDuration, 0.6f* respawnDuration,timer);
            fadeMaterial.color = fadeColor;

            timer += Time.deltaTime;
            yield return null;
        }

        deathScreen.SetActive(false);

        canvas.SetActive(true);
        controller.enabled = true;
        currentState = CharacterState.Grounded;
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

    private void UnlockClimbing()
    {
        climbingUnlocked = true;
        GrandTemple.OnRingExtended -= UnlockClimbing;
        GrandTemple.OnRingExtended += UnlockBasket;
    }

    private void UnlockBasket()
    {
        Debug.Log("Unlocked the berry basket!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * drowningHeight, 0.25f);
        // Interaction
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * interactHeight + interactDistance * transform.forward, interactRadius);
        // Respawn
        Gizmos.color = Color.yellow;
        if (respawnTransform)
        {
            Gizmos.DrawSphere(respawnTransform.position, 0.5f);
        }
    }
}
