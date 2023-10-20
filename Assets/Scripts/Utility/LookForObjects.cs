using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LookForObjects<T>
{
    public static List<T> CheckForObjects(T objectToCheckFor, Vector3 checkFromPosition, float checkingRange)
    {
        List<T> result = new List<T>();
        float distance = checkingRange;
        Collider nearest = null;

        foreach (Collider c in Physics.OverlapSphere(checkFromPosition, checkingRange))
        {
            if (c.gameObject.TryGetComponent(out objectToCheckFor) && (c.transform.position - checkFromPosition).sqrMagnitude < distance)
            {
                result.Add(objectToCheckFor); 
                distance = (c.transform.position - checkFromPosition).sqrMagnitude;
                nearest = c;
            }
        }
        return result;
    }

    public static bool TryGetClosestObject(T objectToCheckFor, Vector3 checkFromPosition, float checkingRange, out T nearestObject)
    {
        nearestObject = objectToCheckFor;
        float distance = checkingRange;
        Collider nearest = null;

        foreach (Collider c in Physics.OverlapSphere(checkFromPosition, checkingRange))
        {
            if (c.gameObject.TryGetComponent(out objectToCheckFor) && (c.transform.position - checkFromPosition).sqrMagnitude < distance)
            {
                nearestObject = objectToCheckFor;
                distance = (c.transform.position - checkFromPosition).sqrMagnitude;
                nearest = c;
            }
        }

        return nearest!= null;
    }

}
