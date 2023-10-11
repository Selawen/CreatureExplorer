using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Move : Action
{
    private NavMeshAgent moveAgent;

    private void Start()
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();

        moveAgent.SetDestination(target.transform.position);

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
        await CheckDistanceToDestination();

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
