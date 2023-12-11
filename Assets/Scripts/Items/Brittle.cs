using System.Collections;
using UnityEngine;

public class Brittle : MonoBehaviour, IBreakable
{
    public void Break()
    {
        // TODO: make object break properly
        if (Vector3.Dot(transform.up, Vector3.up) > 0.5f)
            StartCoroutine(FallOver(2));
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Color originalGizmoColour = Gizmos.color;
        Gizmos.color = Color.red;

        foreach (MeshFilter drawnMesh in GetComponentsInChildren<MeshFilter>())
        {
            Gizmos.DrawWireMesh(drawnMesh.sharedMesh, Vector3.zero, Quaternion.identity, drawnMesh.transform.lossyScale * 1.1f);
        }

        Gizmos.color = originalGizmoColour;
        Gizmos.matrix = originalMatrix;
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
