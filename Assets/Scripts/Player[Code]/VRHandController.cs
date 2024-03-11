using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class VRHandController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onLookAtPalm;
    [SerializeField] private UnityEvent onLookFromPalm;
    [SerializeField] private UnityEvent onPalmsParallel;
    [SerializeField] private UnityEvent onPalmsUnaligned;

    [Header("Settings")]
    [SerializeField] private VRHandController otherHand;
    [SerializeField] private LayerMask PointingInteractionLayers;
    [SerializeField] private float lookAtPalmAngle = 45;
    [SerializeField] private float lookFromPalmAngle = 60;
    [SerializeField] private float palmAlignmentAccuracy = 0.8f;

    private bool lookingAtPalm = false;
    private bool palmsParallel = false;
    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
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
        float handRotationAngle = Vector3.Angle(transform.up, transform.position - Camera.main.transform.position);
        //Debug.Log(Vector3.Angle(transform.up, transform.position - Camera.main.transform.position));
        if (!lookingAtPalm && !palmsParallel)
        {
            if (handRotationAngle < lookAtPalmAngle)
            {
                lookingAtPalm = true;
                onLookAtPalm?.Invoke();
            }
            else if (onPalmsParallel.GetPersistentEventCount()>0)
            {
                if(Vector3.Dot(transform.up, otherHand.transform.up) > palmAlignmentAccuracy && Vector3.Dot(transform.forward, Vector3.up) > palmAlignmentAccuracy && Vector3.Dot(otherHand.transform.forward, Vector3.up) > palmAlignmentAccuracy)
                onPalmsParallel?.Invoke();
                palmsParallel = true;
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
            if (Vector3.Dot(transform.up, otherHand.transform.up) < palmAlignmentAccuracy || Vector3.Dot(transform.forward, Vector3.up) < palmAlignmentAccuracy || Vector3.Dot(otherHand.transform.forward, Vector3.up) < palmAlignmentAccuracy)
            {
                onPalmsUnaligned?.Invoke();
                palmsParallel = false;
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
                    uiElement.Select();
                }
            }
        }
    }

    public void PressTrigger(InputAction.CallbackContext callbackContext)
    {
        if (line == null || !line.enabled)
            return;

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
