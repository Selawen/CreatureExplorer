using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private List<T> poolObjects;

    public bool GetFromPool(out T pooledItem)
    {
        if (poolObjects.Count == 0)
        {
            pooledItem = null;
            return false;
        }
        pooledItem = poolObjects[0];
        poolObjects.Remove(pooledItem);
        return true;
    }

    public void AddToPool(T pooledItem)
    {
        poolObjects.Add(pooledItem);
    }

}
