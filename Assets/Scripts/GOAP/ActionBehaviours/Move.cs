using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Move : Action
{
    private NavMeshAgent moveAgent;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = creature.GetComponent<NavMeshAgent>();
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

        finished = true;
        yield return null;
    }
}
