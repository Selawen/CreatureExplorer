using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float Loudness { get; private set; }

    [SerializeField] private float maximumViewAngle = 70f;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private float interactHeight = 0.875f;
    [SerializeField] private float interactionRadius = 1.25f;
    [SerializeField] private float climbDistance = 0.25f;

    [SerializeField] private GameSettings gameSettings;

    [SerializeField] private LayerMask interactionLayers;

    [SerializeField] private UnityEvent onScrapbookOpened;
    [SerializeField] private UnityEvent onCameraOpened;
    [SerializeField] private UnityEvent onCameraClosed;

    [SerializeField] private UnityEvent<string> onInteractableFound;
    [SerializeField] private UnityEvent onInteractableOutOfRange;

    //[SerializeField] private Camera pictureCamera;

    private float verticalRotation;

    private Vector2 rotationInput;

    private FiniteStateMachine stateMachine;

    private Camera firstPersonCamera;

    private PlayerInput playerInput;

    private IInteractable interactableInRange;

    private void Awake()
    {
        stateMachine = new FiniteStateMachine(typeof(WalkingState), GetComponents<IState>());
        firstPersonCamera = Camera.main;
        verticalRotation = firstPersonCamera.transform.eulerAngles.x;

        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        onCameraClosed?.Invoke();
    }

    // Update is called once per frame
    private void Update()
    {
        stateMachine.OnUpdate();
        HandleRotation(rotationInput);
        HandleInteract();

        //if (Physics.SphereCast(transform.position, interactionRadius, transform.forward, out RaycastHit hit, interactionDistance, interactionLayers))
        //{
        //    if (hit.transform.TryGetComponent(out IInteractable interactable))
        //    {
        //        onInteractableFound?.Invoke(interactable.InteractionPrompt);
        //        interactableInRange = interactable;
        //    }
        //}
        //else if (interactableInRange != null)
        //{
        //    interactableInRange = null;
        //    onInteractableOutOfRange?.Invoke();
        //}
    }

    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate();
        firstPersonCamera.transform.rotation = Quaternion.Euler(new Vector3(verticalRotation, transform.eulerAngles.y, 0));
        //pictureCamera.transform.rotation = Quaternion.Euler(new Vector3(verticalRotation, transform.eulerAngles.y, 0));
    }

    public void SwapToCamera(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            playerInput.SwitchCurrentActionMap("Camera");
            onCameraOpened?.Invoke();
        }
    }

    public void SwapFromCamera(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            playerInput.SwitchCurrentActionMap("Overworld");
            onCameraClosed?.Invoke();
        }
    }

    public void GetInteractionInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started && interactableInRange != null)
        {
            if (interactableInRange.GetType() == typeof(JellyfishLadder))
            {
                onInteractableOutOfRange?.Invoke();
                stateMachine.SwitchState(typeof(ClimbingState));
            }
            interactableInRange.Interact();
            
        }
    }

    public void GetRotationInput(InputAction.CallbackContext callbackContext)
    {
        rotationInput = callbackContext.ReadValue<Vector2>();
    }
    
    public void GetCloseScrapbookInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            playerInput.SwitchCurrentActionMap("Overworld");
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void GetOpenScrapbookInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            playerInput.SwitchCurrentActionMap("Scrapbook");
            onScrapbookOpened?.Invoke();
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetLoudness(float newLoudness) => Loudness = newLoudness;

    private void HandleRotation(Vector2 lookInput)
    { 
        verticalRotation = Mathf.Clamp(verticalRotation - (lookInput.y * gameSettings.LookSensitivity), -maximumViewAngle, maximumViewAngle);
        transform.Rotate(new Vector3(0, lookInput.x * gameSettings.LookSensitivity, 0));
    }
    private void HandleInteract()
    {
        System.Type stateType = stateMachine.CurrentState.GetType();

        if (stateType != typeof(WalkingState) && stateType != typeof(FallingState) && stateType != typeof(JumpingState))
        {
            Debug.Log("State is neither walking nor jumping nor falling, returning");
            return;
        }

        interactableInRange = null;

        if (Physics.Raycast(transform.position + Vector3.up * interactHeight, transform.forward, out RaycastHit climb, climbDistance, interactionLayers))
        {
            if (climb.transform.TryGetComponent(out JellyfishLadder ladder))
            {
                ladder.ContactPoint = climb.point;
                interactableInRange = ladder;
                onInteractableFound?.Invoke(interactableInRange.InteractionPrompt);
                return;
            }
        }
        Collider[] collisions = Physics.OverlapSphere(transform.position + transform.forward * interactionDistance + Vector3.up * interactHeight, interactionRadius, interactionLayers);
        if (collisions.Length > 0)
        {
            Collider closest = null;
            foreach (Collider c in collisions)
            {
                // First, we check if the collisions we found can actually be seen from the player's perspective and aren't obscured by another object
                Vector3 interactOrigin = transform.position + Vector3.up * interactHeight;
                if (Physics.Raycast(interactOrigin, c.transform.position - interactOrigin, out RaycastHit hit, interactionDistance, interactionLayers))
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
                        interactableInRange = interactable;
                    }
                }
            }
        }

        if (interactableInRange != null)
        {
            onInteractableFound?.Invoke(interactableInRange.InteractionPrompt);
            return;
        }
        onInteractableOutOfRange?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * interactionDistance) + Vector3.up * interactHeight, interactionRadius);
    }
}
