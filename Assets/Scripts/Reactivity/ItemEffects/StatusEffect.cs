using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private string StatusEffectName;

    [SerializeField] private CreatureState statusEffect;

    private float effectDuration = 0;
    private Creature affectedCreature;

    public StatusEffect(Creature toAffect, float duration = 0)
    {
        effectDuration = duration;
        affectedCreature = toAffect;

        StartCoroutine(TriggerEffect());
    }

    private IEnumerator TriggerEffect()
    {
        float timer = 0;

        do
        {
            affectedCreature.UpdateValues(statusEffect);
            timer += Time.deltaTime;

            yield return null;
        } while (timer < effectDuration);

        DestroyImmediate(this);
    }
}
