using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Ram : Action
{
    [Header("Ram")]
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

    public override GameObject PerformAction(Creature creature, GameObject target)
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

        //Task.Run(() => DoAction(), token);
        DoAction(target);
        FailCheck(failToken);

        return target;
    }

    public override void CalculateCostAndReward(CreatureState currentState, MoodState targetMood, float targetMoodPrio)
    {
        base.CalculateCostAndReward(currentState, targetMood, targetMoodPrio);
    }

    public override void Reset()
    {
        base.Reset();

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = true;

        moveAgent.ResetPath();
    }

    protected override async void DoAction(GameObject target = null)
    {
        await CheckDistanceToDestination();

        if (target.TryGetComponent(out IBreakable broken))
        {
            broken.Break();
        }

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = true;

        moveAgent.ResetPath();

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

