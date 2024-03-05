using UnityEngine;

public class AreaSpawner : Spawner
{
    protected override void Spawn()
    {
        Vector3 spawnpos = transform.position + new Vector3(Random.Range(-spawnRange, spawnRange), 0, Random.Range(-spawnRange, spawnRange));

        if (Physics.Raycast(spawnpos + Vector3.up * 200, Vector3.down, out RaycastHit hit, 500, canspawnOn))
        {
            Vector3 spawnPoint;

            // place underneath trees when spawning on terrain
            if (hit.collider.gameObject.TryGetComponent(out Terrain terrain))
                spawnPoint = new Vector3(hit.point.x, terrain.SampleHeight(hit.point) + hit.transform.position.y, hit.point.z);
            else
                spawnPoint = hit.point;

            Instantiate(spawnedObject, spawnPoint + new Vector3(0, spawnedObject.transform.lossyScale.y * 0.5f, 0), transform.rotation, transform);
        }
    }
}
