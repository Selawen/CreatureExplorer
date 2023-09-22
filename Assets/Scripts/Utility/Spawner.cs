using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnedObject;

    [SerializeField] private float spawnrange;
    [SerializeField] private float spawnDelay;
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

        Instantiate(spawnedObject, transform.position + new Vector3(Random.Range(0, spawnrange), 0, Random.Range(0, spawnrange)), transform.rotation);

        if (continous)
            StartCoroutine(Spawn());
    }
}
