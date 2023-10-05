using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Ram : Action
{
    [SerializeField] private float speedMultiplier = 4;

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

        moveAgent.speed *= speedMultiplier;
        moveAgent.angularSpeed *= speedMultiplier;
        moveAgent.acceleration *= speedMultiplier;
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
        moveAgent.SetDestination(moveAgent.transform.position);
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

