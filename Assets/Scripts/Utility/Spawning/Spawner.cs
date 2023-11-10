using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Gizmos")]
    [SerializeField] protected Color gizmoColour;

    [Header("Spawning")]
    [SerializeField] protected LayerMask canspawnOn;
    [SerializeField] protected GameObject spawnedObject;

    [SerializeField] protected float spawnrange;
    [SerializeField] protected float spawnDelay;
    [SerializeField] protected int maxSpawnAmount = 10;
    [SerializeField] protected bool continous;


    // Start is called before the first frame update
    private void Awake()
    {
        Spawn();
        StartCoroutine(SpawnTimer());
    }

    protected virtual void Spawn()
    {
        Vector3 spawnpos = transform.position + new Vector3(Random.Range(-spawnrange, spawnrange), 0, Random.Range(-spawnrange, spawnrange));

        if (Physics.Raycast(spawnpos + Vector3.up * 200, Vector3.down, out RaycastHit hit, 500, canspawnOn))
        {
            Instantiate(spawnedObject, hit.point + new Vector3(0, spawnedObject.transform.lossyScale.y * 0.5f, 0), transform.rotation, transform);
        }
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnDelay);

        while (transform.childCount > maxSpawnAmount)
        {
            yield return null;
        }

        Spawn();

        if (continous)
            StartCoroutine(SpawnTimer());
    }

    protected virtual void OnDrawGizmos()
    {
        Color originalColour = Gizmos.color;
        Gizmos.color = gizmoColour;
        Gizmos.DrawWireSphere(transform.position, (spawnrange+1)); 
        Gizmos.color = originalColour;
    }
}
