using UnityEngine;

public class AnimatedBreakable : MonoBehaviour, IBreakable
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTrigger = "Break";
    [ShowOnly] private bool broken = false;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Break()
    {
        if (broken)
            return;

        animator.SetTrigger(animationTrigger);
        broken = true;
    }


#if UNITY_EDITOR
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
#endif
}