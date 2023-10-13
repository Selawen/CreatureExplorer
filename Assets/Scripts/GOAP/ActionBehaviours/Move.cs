using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Move : Action
{
    private NavMeshAgent moveAgent;
    private Transform targetTransform;

    private void Start()
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    public override GameObject PerformAction(Creature creature, GameObject target)
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
        targetTransform = target.transform;

        moveAgent.SetDestination(targetTransform.position);

        Task.Run(() => DoAction(), failToken);

        // Navmeshagent doesn't play nice with threading
        DoAction();
        FailCheck(failToken);

        return target;
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

        moveAgent.ResetPath();

        base.DoAction();
    }

    private async Task CheckDistanceToDestination()
    {
        while ((moveAgent.destination - moveAgent.transform.position).sqrMagnitude > 1f)
        {
            await Task.Yield();
        }
    }
}
