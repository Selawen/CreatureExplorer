using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : StatusEffect
{
    [SerializeField] private float range;
    [SerializeField] private float pulseTime;

    [Header("Gizmos")]
    [SerializeField] private Color gizmoColour;

    private ParticleSystem particles;


    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        //TODO: Have particle system range follow area range automatically

        StartCoroutine(TimedEffectTrigger());
    }

    private IEnumerator TimedEffectTrigger()
    {
        yield return new WaitForSeconds(pulseTime);

        if (particles!= null)
        {
            particles.Play();
        }

        List<Creature> creaturesInRange = LookForObjects<Creature>.CheckForObjects(transform.position, range);
        TriggerStatusEffect(creaturesInRange);

        StartCoroutine(TimedEffectTrigger());
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        GizmoDrawer.DrawPrimitive(transform.position, Vector3.one * range, GizmoType.WireSphere, gizmoColour);
    }
#endif
}
