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

        DoAction();
        FailCheck(token);

        return target;
    }

    public override void Reset()
    {
        base.Reset();
        moveAgent.SetDestination(moveAgent.transform.position);
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
