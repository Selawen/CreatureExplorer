using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ram : Action
{
    private NavMeshAgent moveAgent;

    private float originalSpeed, originalRotationSpeed, originalAcceleration;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = creature.GetComponent<NavMeshAgent>();
        originalSpeed = moveAgent.speed;
        originalRotationSpeed = moveAgent.angularSpeed;
        originalAcceleration = moveAgent.acceleration;

        moveAgent.speed *= 4;
        moveAgent.angularSpeed *= 4;
        moveAgent.acceleration *= 4;
        moveAgent.autoBraking = false;
        moveAgent.SetDestination(target.transform.position);

        StartCoroutine(CheckFinish());

        return target;
    }

    public override void Reset()
    {
        base.Reset();

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = false;
    }

    protected override IEnumerator CheckFinish()
    {
        while ((moveAgent.destination - moveAgent.transform.position).magnitude > 0.5f)
        {
            yield return null;
        }

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = false;
        finished = true;
        yield return null;
    }
}

