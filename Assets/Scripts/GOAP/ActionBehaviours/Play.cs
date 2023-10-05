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

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        moveAgent = gameObject.GetComponentInParent<NavMeshAgent>();
        DoAction();
        FailCheck(token);
        return target;
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
