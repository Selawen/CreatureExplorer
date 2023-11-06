using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Roam : Action
{
    [SerializeField] private float roamRange;
    private NavMeshAgent moveAgent;
    private void Start()
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    public override GameObject PerformAction(Creature creature, GameObject target)
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();

        moveAgent.SetDestination(creature.transform.position + Random.insideUnitSphere * Random.Range(1,roamRange));

        //Task.Run(() => DoAction(), failToken);

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
        Task[] check = { CheckDistanceToDestination(), Task.Delay((int)(actionDuration *1000))};
        await Task.WhenAny(check);
        {
            moveAgent.ResetPath();

            base.DoAction();
        }
    }

    private async Task CheckDistanceToDestination()
    {
        while ((moveAgent.destination - moveAgent.transform.position).sqrMagnitude > 1f)
        {
            await Task.Yield();
        }
    }
}
