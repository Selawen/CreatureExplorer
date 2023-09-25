using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ram : Action
{
    private NavMeshAgent moveAgent;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = creature.GetComponent<NavMeshAgent>();
        moveAgent.speed *= 4;
        moveAgent.acceleration *= 2;
        moveAgent.SetDestination(target.transform.position);

        StartCoroutine(CheckFinish());

        return target;
    }

    protected override IEnumerator CheckFinish()
    {
        while ((moveAgent.destination - moveAgent.transform.position).magnitude > 0.5f)
        {
            yield return null;
        }

        moveAgent.speed *= 0.25f;
        moveAgent.acceleration *= 0.5f;
        finished = true;
        yield return null;
    }
}

