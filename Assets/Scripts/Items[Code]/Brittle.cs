using System.Collections;
using UnityEngine;

public class Brittle : MonoBehaviour, IBreakable
{
    [field: SerializeField] private AudioClip breakingSound;
    private SoundPlayer soundPlayer;

    public void Break()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        if (soundPlayer == null)
            soundPlayer = GetComponentInParent<SoundPlayer>();
        
        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(breakingSound, true);
        }

        // TODO: make object break properly
        if (Vector3.Dot(transform.up, Vector3.up) > 0.5f)
            StartCoroutine(FallOver(2));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color originalGizmoColour = Gizmos.color;
        Gizmos.color = Color.red;

        foreach (MeshFilter drawnMesh in GetComponentsInChildren<MeshFilter>())
        {
            Gizmos.DrawWireMesh(drawnMesh.sharedMesh, drawnMesh.transform.position, drawnMesh.transform.rotation, drawnMesh.transform.lossyScale * 1.1f);
        }

        Gizmos.color = originalGizmoColour;
    }
#endif

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
