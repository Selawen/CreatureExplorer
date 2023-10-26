using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private LayerMask raycastMask;

    [ExecuteAlways]
    private void OnValidate()
    {
        if (!(raycastMask.value == 0))
        {
            if (Physics.Raycast(transform.position + Vector3.up * 200, Vector3.down, out RaycastHit hit, 500, raycastMask))
            {
                transform.position = hit.point;
                transform.up = hit.normal;
                transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
            }
        }
    }

}
