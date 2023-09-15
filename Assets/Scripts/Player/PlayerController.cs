using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maximumViewAngle = 70f;

    [SerializeField] private UnityEvent onScrapbookOpened;

    private float verticalRotation;

    private FiniteStateMachine stateMachine;

    private Camera firstPersonCamera;

    private PlayerInput playerInput;

    private void Awake()
    {
        firstPersonCamera = Camera.main;
        verticalRotation = firstPersonCamera.transform.eulerAngles.x;

        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Start()
    {
        stateMachine = new FiniteStateMachine(typeof(WalkingState), GetComponents<IState>());
    }

    // Update is called once per frame
    private void Update()
    {
        stateMachine.OnUpdate();
    }
    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate();
    }

    public void GetRotationInput(InputAction.CallbackContext callbackContext)
    {
        HandleRotation(callbackContext.ReadValue<Vector2>());
    }
    public void GetOpenScrapbookInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            playerInput.SwitchCurrentActionMap("Scrapbook");
            onScrapbookOpened?.Invoke();
        }
    }

    private void HandleRotation(Vector2 lookInput)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - (lookInput.y * mouseSensitivity), -maximumViewAngle, maximumViewAngle);

        transform.Rotate(new Vector3(0, lookInput.x * mouseSensitivity, 0));
        firstPersonCamera.transform.rotation = Quaternion.Euler(new Vector3(verticalRotation, transform.eulerAngles.y, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
