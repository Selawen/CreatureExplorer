using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VRHandController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onLookAtPalm;
    [SerializeField] private UnityEvent onLookFromPalm;
    [SerializeField] private UnityEvent onPalmsParallel;
    [SerializeField] private UnityEvent onPalmsUnaligned;
    [SerializeField] private UnityEvent onFaint;

    [Header("Settings")]
    [SerializeField] private VRHandController otherHand;
    [SerializeField] private LayerMask PointingInteractionLayers;
    [Header("Fainting")]
    [SerializeField] private float faintingPalmAngle = 170;
    [SerializeField] private float sqrDistanceHandToForehead = 0.05f;
    [SerializeField] private float secondsToFaint = 5;
    [SerializeField] private Volume volume;
    [Header("Looking at palm")]
    [SerializeField] private float lookAtPalmAngle = 45;
    [SerializeField] private float lookFromPalmAngle = 60;
    [Header("Palms up and facing each other")]
    [SerializeField] private float palmAlignmentAccuracy = 0.8f;
    [SerializeField] private float handUpAccuracy = 0.7f;
    [SerializeField] private float sqrMaxHandDistance = 0.3f;
    [Header("Grabbing things")]
    [SerializeField] private Vector3 grabOffset;
    [SerializeField] private float grabRadius;

    private Transform cameraTransform;
    private bool lookingAtPalm = false;
    private bool palmsParallel = false;
    private bool holding = false;
    private bool grabbing = false;

    private IGrabbable grabbedObj;

    private float faintingTimer = 0;

    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;

        TryGetComponent(out line);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckHandOrientation();
        if (line != null)
            Point();
    }

    void CheckHandOrientation()
    {
        float handRotationAngle = Vector3.Angle(transform.up, transform.position - cameraTransform.position);

        // check for fainting posture
        if (handRotationAngle > faintingPalmAngle && (transform.position - cameraTransform.position).sqrMagnitude < sqrDistanceHandToForehead)
        {
            faintingTimer += Time.deltaTime;

            if (volume.profile.TryGet(out Vignette vignette))
            {
                vignette.intensity.value = faintingTimer/secondsToFaint;
            }

            if (faintingTimer > secondsToFaint)
            {
                faintingTimer = 0;
                if (vignette != null)
                {
                    vignette.intensity.value = 0;
                }
                onFaint.Invoke();
            }
        }
        else if (faintingTimer != 0)
        {
            faintingTimer = 0;
            if (volume.profile.TryGet(out Vignette vignette))
            {
                vignette.intensity.value = 0;
            }
        }

        if (!lookingAtPalm && !palmsParallel)
        {
            // check for looking at palm posture
            if (handRotationAngle < lookAtPalmAngle)
            {
                lookingAtPalm = true;
                onLookAtPalm?.Invoke();
            }
            // if there is an event to be called when palms are parallel, check whether palms are parallel
            else if (onPalmsParallel.GetPersistentEventCount()>0)
            {
                if (HandsAlignedAndUp() && (transform.position - otherHand.transform.position).sqrMagnitude < sqrMaxHandDistance)
                {
                    onPalmsParallel?.Invoke();
                    palmsParallel = true;
                }
            }
        }
        else if (lookingAtPalm)
        {
            if (handRotationAngle > lookFromPalmAngle)
            {
                lookingAtPalm = false;
                onLookFromPalm?.Invoke();
            }
        }
        else if (palmsParallel)
        {
            if (!HandsAlignedAndUp(0.9f) || (transform.position - otherHand.transform.position).sqrMagnitude > sqrMaxHandDistance)
            {
                palmsParallel = false;
                onPalmsUnaligned?.Invoke();
            }
        }
    }

    private void Point()
    {
        if (line.enabled)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100, PointingInteractionLayers))
            {
                //Debug.Log($"pointing at: {hit.collider.gameObject.name}");
                if (hit.collider.TryGetComponent(out Selectable uiElement))
                {
                    line.SetPosition(1, new Vector3(0, 0, hit.distance));
                    uiElement.Select();
                    return;
                }
            }

            line.SetPosition(1, new Vector3(0, 0, 10));
        }
    }

    public void PressTrigger(InputAction.CallbackContext callbackContext)
    {
        if (line == null || !line.enabled)
            return;

        if (callbackContext.started)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100, PointingInteractionLayers))
            {
                //Debug.Log($"hit {hit.collider.gameObject.name}");
                if (hit.collider.TryGetComponent(out Button button))
                {
                    button.onClick.Invoke();
                }
            }
        }
    }    

    public void PressGrip(InputAction.CallbackContext callbackContext)
    {
        Debug.Log(callbackContext.action.ReadValue<float>());
        if (!grabbing && callbackContext.action.WasPerformedThisFrame())
        {
            grabbing = true;
            Debug.Log("grabbing");
            Collider[] collidersInReach = Physics.OverlapSphere(transform.position + grabOffset, grabRadius);
            foreach (Collider col in collidersInReach)
            {
                if (col.TryGetComponent(out IGrabbable grabbable))
                {
                    grabbedObj = grabbable;

                    grabbedObj.Grab(transform);
                    holding = true;
                }
            }  
        } else if (callbackContext.action.ReadValue<float>() == 0)
        {
            grabbing = false;
            Debug.Log("releasing");
            if (grabbedObj != null)
                grabbedObj.Release();
            holding = false;
        }
    }

    private bool HandsAlignedAndUp(float angleMultiplier = 1)
    {
        return (Vector3.Dot(transform.up, otherHand.transform.up) > palmAlignmentAccuracy * angleMultiplier && 
            Vector3.Dot(transform.forward, cameraTransform.up) > handUpAccuracy * angleMultiplier && 
            Vector3.Dot(otherHand.transform.forward, cameraTransform.up) > handUpAccuracy * angleMultiplier);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        GizmoDrawer.DrawPrimitive(transform.position + grabOffset, Vector3.one * grabRadius, GizmoType.WireSphere, Color.blue);
    }
#endif
}
