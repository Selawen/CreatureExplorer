using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsStepper : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float maxStepHeight = 0.3f;
    [SerializeField, Min(0.1f)] private float maxStepDistance = 0.2f;
    [SerializeField, Min(0.1f)] private float stepSmooth = 0.1f;

    [SerializeField] private LayerMask ignoredStepLayers;

    public void HandleStep(ref Rigidbody rigidbody, Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction,maxStepDistance, ~ignoredStepLayers))
        {
            if (!Physics.Raycast(transform.position + Vector3.up * maxStepHeight, direction, maxStepDistance, ~ignoredStepLayers))
            {
                rigidbody.MovePosition(transform.position + stepSmooth * Time.fixedDeltaTime * Vector3.up);
            }
        }
    }

}
