using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Play : Action
{
    private NavMeshAgent moveAgent;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = creature.GetComponent<NavMeshAgent>();
        moveAgent.SetDestination(creature.transform.position + (creature.transform.forward + creature.transform.right));

        StartCoroutine(CheckFinish());
        return target;
    }

    protected override IEnumerator CheckFinish()
    {
        float playTimer = actionDuration;

        while (playTimer > 0)
        {
            // TODO: make more performant
            moveAgent.SetDestination(moveAgent.transform.position + (moveAgent.transform.forward + moveAgent.transform.right));

            playTimer -= Time.deltaTime;
            yield return null;
        }
        finished = true;
        yield return null;
    }
}
