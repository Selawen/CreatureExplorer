using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnGround : MonoBehaviour
{
    [SerializeField] private bool reset;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private bool randomRotate;

    [SerializeField] private LayerMask raycastMask;

    [ExecuteAlways]
    private void OnValidate()
    {
        if (!(raycastMask.value == 0) && gameObject.activeInHierarchy)
        {
            reset = false;
            if (Physics.Raycast(transform.position + Vector3.up * 200, Vector3.down, out RaycastHit hit, 500, raycastMask))
            {
                transform.position = hit.point + offset;
                transform.up = hit.normal;

                if (randomRotate)
                    transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
            }
        }
    }
}
