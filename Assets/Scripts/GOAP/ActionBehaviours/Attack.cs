using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Attack : Action
{
    [SerializeField] private float speedMultiplier = 4;

    private NavMeshAgent moveAgent;
    private Transform targetTransform;

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
        if (target != null)
        {
            moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
            originalSpeed = moveAgent.speed;
            originalRotationSpeed = moveAgent.angularSpeed;
            originalAcceleration = moveAgent.acceleration;

            moveAgent.speed *= speedMultiplier;
            moveAgent.angularSpeed *= speedMultiplier;
            moveAgent.acceleration *= speedMultiplier;
            moveAgent.autoBraking = false;

            targetTransform = target.transform;
            moveAgent.SetDestination(targetTransform.position);
        }
        else
        {
            failed = true;
            return target;
        }

        DoAction(target);
        FailCheck(failToken);
        return target;
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

    public override void CalculateCostAndReward(CreatureState currentState, MoodState targetMood, float targetMoodPrio)
    {
        base.CalculateCostAndReward(currentState, targetMood, targetMoodPrio);
    }

    protected override async void DoAction(GameObject target = null)
    {
        Task check = CheckDistanceToDestination();

        while (!check.IsCompletedSuccessfully)
        {
            if ((moveAgent.destination - targetTransform.position).sqrMagnitude > 1f)
            {
                moveAgent.SetDestination(targetTransform.position);
            }
            // wait half a second before updating again
            await Task.Delay(500);
        }

        moveAgent.speed = originalSpeed;
        moveAgent.angularSpeed = originalRotationSpeed;
        moveAgent.acceleration = originalAcceleration;
        moveAgent.autoBraking = true;

        moveAgent.ResetPath();
        //moveAgent.SetDestination(moveAgent.transform.position);

        // TODO: implement attack on player
        if (target.TryGetComponent(out Creature targetCreature))
        {
            if (targetCreature.AttackSuccess(moveAgent.transform.position))
            {
                if (target.TryGetComponent(out NavMeshAgent agent))
                {
                    agent.enabled = false;
                }
            } else
            {
                if (GetComponentInParent<Creature>().LogDebugs)
                    Debug.Log("Attack failed");

                failed = true;
                failSource.Cancel();
                return;
            }
        }
        base.DoAction();
    }
    private async Task CheckDistanceToDestination()
    {
        while ((moveAgent.destination - moveAgent.transform.position).magnitude > 1f)
        {
            await Task.Yield();
        }
    }
}
