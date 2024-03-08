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

    [Header("Settings")]
    [SerializeField] private LayerMask PointingInteractionLayers;
    [SerializeField] private float lookAtPalmAngle = 45;
    [SerializeField] private float lookFromPalmAngle = 60;

    private bool lookingAtPalm = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckHandOrientation();
        Point();
    }

    void CheckHandOrientation()
    {
        float handRotationAngle = Vector3.Angle(transform.up, transform.position - Camera.main.transform.position);
        //Debug.Log(Vector3.Angle(transform.up, transform.position - Camera.main.transform.position));
        if (!lookingAtPalm )
        {
            if (handRotationAngle < lookAtPalmAngle)
            {
                lookingAtPalm = true;
                onLookAtPalm?.Invoke();
            }
        } 
        else if (handRotationAngle > lookFromPalmAngle)
        {
            lookingAtPalm = false;
            onLookFromPalm?.Invoke();
        }
    }

    private void Point()
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

        public void PressTrigger(InputAction.CallbackContext callbackContext)
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
