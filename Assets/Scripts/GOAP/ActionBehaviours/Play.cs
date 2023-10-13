using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Play : Action
{
    private NavMeshAgent moveAgent;

    private void Start()
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    public override GameObject PerformAction(Creature creature, GameObject target)
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
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
        float playTimer = actionDuration;

        while (playTimer > 0)
        {
            // TODO: make more performant
            moveAgent.SetDestination(moveAgent.transform.position + (moveAgent.transform.forward + moveAgent.transform.right));

            playTimer -= Time.deltaTime;
            await Task.Yield();
        }

        base.DoAction();
    }
}
