using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Flee : Action
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
        moveAgent.SetDestination(creature.transform.position +(creature.transform.position - creature.GetComponent<Creature>().waryOff).normalized*20);

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
    }

    protected override async void DoAction(GameObject target = null)
    {
        //Task[] tasks = {Task.Delay((int)(actionDuration * 1000)), CheckDistanceToDestination()};

        await Task.Delay((int)(actionDuration * 1000));

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;

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
