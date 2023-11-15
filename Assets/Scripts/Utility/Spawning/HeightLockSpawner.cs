using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightLockSpawner : Spawner
{
    [Tooltip("how many meters above and below the spawner may the object be spawned")]
    [SerializeField] private float tolerance = 5;

    protected override void Spawn()
    {
        Vector3 spawnpos = transform.position + new Vector3(Random.Range(-spawnrange, spawnrange), 0, Random.Range(-spawnrange, spawnrange));

        if (Physics.Raycast(spawnpos + Vector3.up * tolerance, Vector3.down, out RaycastHit hit, 2*tolerance, canspawnOn))
        {
            Instantiate(spawnedObject, hit.point + new Vector3(0, spawnedObject.transform.lossyScale.y * 0.5f, 0), transform.rotation, transform);
        }
    }
}
