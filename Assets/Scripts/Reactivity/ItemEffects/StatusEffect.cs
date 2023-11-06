using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] protected string StatusEffectName;
    [SerializeField] protected bool oneShot;
    [SerializeField] protected float effectDuration = 0;

    [SerializeField] private CreatureState statusEffect;

    private Creature affectedCreature;

    private void OnValidate()
    {
        if (!oneShot && effectDuration == 0)
        {
            Debug.LogError($"Effect of {gameObject.name} is set to have an effect over time, but the duration is set to 0");
        }
    }

    protected void TriggerStatusEffect(Creature toAffect)
    {
        affectedCreature = toAffect;

        if (oneShot)
        {
            affectedCreature.UpdateValues(statusEffect);
            DestroyImmediate(this);
        }
        else
            StartCoroutine(TriggerEffect());
    }

    public void TriggerStatusEffect(Creature toAffect, float duration = -1, bool instantanious = false)
    {
        effectDuration = duration < 0? effectDuration: duration;
        affectedCreature = toAffect;

        if (oneShot || instantanious)
        {
            affectedCreature.UpdateValues(statusEffect);
            DestroyImmediate(this);
        }
        else
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
