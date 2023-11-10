using UnityEngine;

public class Edible : StatusEffect
{
    private bool eaten = false;

    private Creature eatenBy;

    public void StartEating(Creature creature)
    {
        eaten = true;
        eatenBy = creature;
    }

    private void OnDestroy()
    {
        if (eatenBy != null)
            TriggerStatusEffect(eatenBy);
    }
}