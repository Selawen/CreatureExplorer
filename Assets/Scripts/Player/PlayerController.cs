using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;

    private bool isCrouching;

    private FiniteStateMachine stateMachine;

    private Camera firstPersonCamera;

    private void Awake()
    {
        firstPersonCamera = Camera.main;

        stateMachine = new FiniteStateMachine(typeof(WalkingState), GetComponents<IState>());

        stateMachine.AddTransition(typeof(WalkingState), typeof(CrouchingState), PlayerIsCrouching);
        stateMachine.AddTransition(typeof(CrouchingState), typeof(WalkingState), PlayerIsStanding);

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
        HandleRotation(callbackContext.ReadValue<Vector2>().x);
    }
    public bool PlayerIsCrouching()
    {
        return isCrouching;
    }
    public bool PlayerIsStanding()
    {
        return !isCrouching;
    }
    private void HandleRotation(float horizontalDelta)
    {
        transform.Rotate(new Vector3(0, horizontalDelta * mouseSensitivity, 0));
        firstPersonCamera.transform.rotation = transform.rotation;
    }
}
