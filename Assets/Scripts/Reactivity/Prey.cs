using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Creature
{
    [SerializeField] private float checkForThreatsTimer = 0.5f;
    [SerializeField] private CreatureState reactionToPredator;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CheckForPredators());
    }

    protected void ReactToThreat(Vector3 threatPosition)
    {
        WaryOff = threatPosition;
        SetConditionTrue(worldState, Condition.IsNearDanger);
    }

    protected void ReactToThreat(Vector3 threatPosition, CreatureState reaction)
    {
        UpdateValues(reaction);
        WaryOff = threatPosition;
        SetConditionTrue(worldState, Condition.IsNearDanger);
    }
    
    protected void ReactToThreat(Torca predator)
    {
        ReactToThreat(predator.transform.position, reactionToPredator);
    }

    private IEnumerator CheckForPredators()
    {
        yield return new WaitForSeconds(checkForThreatsTimer);

        float distance = hearingSensitivity;
        Collider nearest = null;

        foreach (Collider c in Physics.OverlapSphere(transform.position, 40))
        {
               if (c.gameObject.TryGetComponent(out Torca predator) && (c.transform.position - transform.position).sqrMagnitude < distance)
               {
                   distance = (c.transform.position - transform.position).sqrMagnitude;
                   nearest = c;
               }
        }

        if (nearest != null)
            ReactToThreat(nearest.gameObject.GetComponent<Torca>());

        StartCoroutine(CheckForPredators());
    }

}
