using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public void Break()
    {
        // TODO: make object break properly
        Debug.Log($"dot is {Vector3.Dot(transform.up, Vector3.up)}");
        if (Vector3.Dot(transform.up, Vector3.up) > 0.5f)
            StartCoroutine(FallOver(2));
    }

    private void OnDrawGizmos()
    {
        Color originalGizmoColour = Gizmos.color;
        Gizmos.color = Color.red;

        MeshFilter drawnMesh = GetComponentInChildren<MeshFilter>();
        Gizmos.DrawWireMesh(drawnMesh.sharedMesh, drawnMesh.transform.position, drawnMesh.transform.rotation, drawnMesh.transform.lossyScale*1.1f);

        Gizmos.color = originalGizmoColour;
    }

    private IEnumerator FallOver(float duration)
    {
        float timer = 0;
        Vector3 from = transform.up;
        Vector3 to = transform.right;

        while (timer < duration)
        {
            transform.up = Vector3.Lerp(from, to, timer / duration);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
