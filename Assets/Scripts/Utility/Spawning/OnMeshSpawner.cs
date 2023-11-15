using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class OnMeshSpawner : Spawner
{
    [SerializeField] private MeshFilter spawnOn;
    [SerializeField] private float distanceFromMesh = 0;

    private MeshCollider spawnCollider;

    private void OnValidate()
    {
        if (spawnOn != null) 
        {
            spawnCollider = GetComponent<MeshCollider>();
            spawnCollider.convex = true;
            spawnCollider.isTrigger = true;
            spawnCollider.sharedMesh = spawnOn.sharedMesh;
            transform.position = spawnOn.transform.position;
        }
    }

    protected override void Spawn()
    {
        // TODO: figure out better way to have objects spawn on mesh (not inside)
        Vector3 spawnpos = transform.position + new Vector3(Random.Range(-spawnrange, spawnrange), Random.Range(-spawnrange, spawnrange), Random.Range(-spawnrange, spawnrange))*10;
        Vector3 spawnPoint = spawnCollider.ClosestPoint(spawnpos);
        spawnPoint = transform.position + (spawnPoint - transform.position) * (1+distanceFromMesh);

        Instantiate(spawnedObject, spawnPoint, transform.rotation, transform);
    }

    protected override void OnDrawGizmos()
    {
        Color originalColour = Gizmos.color;
        Gizmos.color = gizmoColour;
        Gizmos.DrawWireMesh(spawnOn.sharedMesh, transform.position, transform.rotation, transform.lossyScale + transform.lossyScale * distanceFromMesh);
        Gizmos.color = originalColour;
    }
}
