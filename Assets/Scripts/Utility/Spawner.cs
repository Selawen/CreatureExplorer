using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private LayerMask canspawnOn;
    [SerializeField] private GameObject spawnedObject;

    [SerializeField] private float spawnrange;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxSpawnAmount = 10;
    [SerializeField] private bool continous;

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(spawnedObject, transform.position + new Vector3(Random.Range(0, spawnrange), 0, Random.Range(0, spawnrange)), transform.rotation);
        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnDelay);

        while (transform.childCount > maxSpawnAmount)
        {
            yield return null;
        }

        if (continous)
            StartCoroutine(Spawn());

        Vector3 spawnpos = transform.position + new Vector3(Random.Range(-spawnrange, spawnrange), 0, Random.Range(-spawnrange, spawnrange));

        if (Physics.Raycast(spawnpos + Vector3.up * 200, Vector3.down, out RaycastHit hit, 500, canspawnOn))
        {
            Instantiate(spawnedObject, hit.point + new Vector3(0, spawnedObject.transform.lossyScale.y * 0.5f, 0), transform.rotation, transform);
        }
    }
}
