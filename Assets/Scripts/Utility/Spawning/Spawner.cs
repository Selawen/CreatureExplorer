using System.Collections;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
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


    private void Start()
    {
        Spawn();
        StartCoroutine(SpawnTimer());
    }

    protected abstract void Spawn();

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
