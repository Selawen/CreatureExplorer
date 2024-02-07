using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceSurface : MonoBehaviour
{
    [SerializeField, Min(1)] private float minimumEntryForce = 4f;
    [SerializeField, Min(1)] private float defaultExitForce = 4f;

    [Tooltip("Value multiplied with the incoming force to calculate exiting bounce force.")]
    [SerializeField] private float bounceModifier = 1f;

    [SerializeField] private float minimumExitForce = 4f;
    [SerializeField] private float maximumExitForce = 10f;

    public bool Bounce(float entryForce, out float exitForce)
    {
        exitForce = 0;
        if(entryForce >= minimumEntryForce)
        {
            exitForce = Mathf.Clamp(defaultExitForce + ((entryForce - minimumEntryForce) * bounceModifier), minimumExitForce, maximumExitForce);
            return true;
        }
        return false;
    }
}
