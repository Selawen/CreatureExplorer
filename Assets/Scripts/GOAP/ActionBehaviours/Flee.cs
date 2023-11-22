using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Flee : Action
{
    [Header("Flee")]
    [SerializeField] private float speedMultiplier = 4;
    [SerializeField] private float runDistance= 10;

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

        Vector3 fearsource = creature.GetComponent<Creature>().WaryOff;
        if (fearsource == null)
        {
            fearsource = transform.position - transform.forward;
        }

        moveAgent.SetDestination(creature.transform.position +(creature.transform.position - fearsource).normalized*runDistance);

        animator.speed = moveAgent.speed / originalSpeed;

        //Task.Run(() => DoAction(), failToken);
        // Navmeshagent doesn't play nice with threading
        DoAction();
        FailCheck(failToken);

        return target;
    }

    public override void Reset()
    {
        base.Reset();

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;

        animator.speed = 1;
    }

    public override void CalculateCostAndReward(CreatureState currentState, MoodState targetMood, float targetMoodPrio)
    {
        base.CalculateCostAndReward(currentState, targetMood, targetMoodPrio);
    }

    protected override async void DoAction(GameObject target = null)
    {
        Task[] tasks = {Task.Delay((int)(actionDuration * 1000)), CheckDistanceToDestination()};

        await Task.WhenAny(tasks);// .Delay((int)(actionDuration * 1000));
        {
            moveAgent.speed = originalSpeed;
            moveAgent.angularSpeed = originalRotationSpeed;
            moveAgent.acceleration = originalAcceleration;

            animator.speed = 1;

            base.DoAction();
        }
    }

    private async Task CheckDistanceToDestination()
    {
        while ((moveAgent.destination - moveAgent.transform.position).magnitude > 0.5f)
        {
            await Task.Yield();
        }
    }
}
