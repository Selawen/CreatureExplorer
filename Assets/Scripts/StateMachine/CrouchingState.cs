using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CrouchingState : State
{
    [SerializeField] private float sneakSpeed = 3f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchEyeHeight = 0.8f;

    [SerializeField] private Camera firstPersonCamera;

    private Vector2 moveInput;

    private new Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public override void OnStateEnter()
    {
        capsuleCollider.height = crouchHeight;
        capsuleCollider.center = Vector3.up * (crouchHeight * 0.5f);
    }
    public override void OnStateUpdate()
    {
        firstPersonCamera.transform.position = new(transform.position.x, crouchEyeHeight, transform.position.z);
    }
    public override void OnStateFixedUpdate()
    {
        if (moveInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + rigidbody.transform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            rigidbody.MovePosition(rigidbody.transform.position + (sneakSpeed * Time.fixedDeltaTime * moveDirection.normalized));
        }
    }
    public void GetMoveInput(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>().normalized;
    }
}
