using UnityEngine;

public class AnimatedBreakable : MonoBehaviour, IBreakable
{

    [field: SerializeField] private AudioClip breakingSound;
    [field: SerializeField] private AudioClip AnimationEndSound;
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTrigger = "Break";
    [ShowOnly] private bool broken = false;
    [SerializeField] private bool showGizmo = true;

    private SoundPlayer soundPlayer;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        soundPlayer = GetComponent<SoundPlayer>();
        if (soundPlayer == null)
            soundPlayer = GetComponentInParent<SoundPlayer>();
    }

    public void Break()
    {
        if (broken)
            return;

        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(breakingSound, true);
        }

        animator.SetTrigger(animationTrigger);
        broken = true;
    }

    public void PlaySoundOnAnimationEnd()
    {
        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(AnimationEndSound, true);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmo)
            return;

        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Color originalGizmoColour = Gizmos.color;
        Gizmos.color = Color.red;

        foreach (MeshFilter drawnMesh in GetComponentsInChildren<MeshFilter>())
        {
            Gizmos.DrawWireMesh(drawnMesh.sharedMesh, Vector3.zero, Quaternion.identity, drawnMesh.transform.localScale * 1.1f);
        }

        Gizmos.color = originalGizmoColour;
        Gizmos.matrix = originalMatrix;
    }
#endif
}