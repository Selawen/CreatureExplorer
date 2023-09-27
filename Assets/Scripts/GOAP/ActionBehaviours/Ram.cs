using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Ram : Action
{
    private NavMeshAgent moveAgent;

    private float originalSpeed, originalRotationSpeed, originalAcceleration;

    private void Start()
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
        originalSpeed = moveAgent.speed;
        originalRotationSpeed = moveAgent.angularSpeed;
        originalAcceleration = moveAgent.acceleration;
    }

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();

        originalSpeed = moveAgent.speed;
        originalRotationSpeed = moveAgent.angularSpeed;
        originalAcceleration = moveAgent.acceleration;

        moveAgent.speed *= 4;
        moveAgent.angularSpeed *= 4;
        moveAgent.acceleration *= 4;
        moveAgent.autoBraking = false;
        moveAgent.SetDestination(target.transform.position);

        DoAction();
        FailCheck(token);

        return target;
    }

    public override void Reset()
    {
        base.Reset();

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = true;
    }

    protected override async void DoAction(GameObject target = null)
    {
        await CheckDistanceToDestination();

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = true;

        base.DoAction();
    }

    private async Task CheckDistanceToDestination()
    {
        while ((moveAgent.destination - moveAgent.transform.position).magnitude > 0.5f)
        {
            await Task.Yield();
        }
    }
}

