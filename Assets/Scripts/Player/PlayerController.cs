using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maximumViewAngle = 70f;

    private float verticalRotation;

    private bool isCrouching;

    private FiniteStateMachine stateMachine;

    private Camera firstPersonCamera;

    private void Awake()
    {
        firstPersonCamera = Camera.main;
        verticalRotation = firstPersonCamera.transform.eulerAngles.x;

        stateMachine = new FiniteStateMachine(typeof(WalkingState), GetComponents<IState>());

        stateMachine.AddTransition(typeof(WalkingState), typeof(CrouchingState), PlayerIsCrouching);
        stateMachine.AddTransition(typeof(CrouchingState), typeof(WalkingState), PlayerIsStanding);


        Cursor.lockState = CursorLockMode.Locked;
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

    public void GetCrouchInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            isCrouching = !isCrouching;
        }
    }
    public void ReceiveRotationInput(InputAction.CallbackContext callbackContext)
    {
        HandleRotation(callbackContext.ReadValue<Vector2>());
    }
    public bool PlayerIsCrouching()
    {
        return isCrouching;
    }
    public bool PlayerIsStanding()
    {
        return !isCrouching;
    }
    private void HandleRotation(Vector2 lookInput)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - (lookInput.y * mouseSensitivity), -maximumViewAngle, maximumViewAngle);

        transform.Rotate(new Vector3(0, lookInput.x * mouseSensitivity, 0));
        firstPersonCamera.transform.rotation = Quaternion.Euler(new Vector3(verticalRotation, transform.eulerAngles.y, 0));
    }
}
