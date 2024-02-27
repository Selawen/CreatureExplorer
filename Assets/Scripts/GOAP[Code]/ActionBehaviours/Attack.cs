using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Attack : NavigatedAction
{
    [Header("Attack")]
    [SerializeField] private Transform attackOrigin;

    public override GameObject PerformAction(Creature creature, GameObject target)
    {
        if (target != null)
        {
            base.PerformAction(creature, target);
        }
        else
        {
            failed = true;
            return target;
        }

        return target;
    }

    protected override void SetPathDestination()
    {
        moveAgent.SetDestination(targetTransform.position);
    }

    protected override void MoveAction(GameObject target = null)
    {
        DoAction(target);
    }

    protected override async void DoAction(GameObject target = null)
    {
        float comparison = 2f;

        if (target.TryGetComponent(out NavMeshAgent agent))
        {
            comparison += agent.radius;
        }
        Task check = CheckDistanceToDestination(comparison);

        while (!check.IsCompletedSuccessfully)
        {
            // wait a bit before updating
            await Task.Delay(100);

            if (creatureDeactivated)
                return;

            if ((moveAgent.destination - targetTransform.position).sqrMagnitude > 1f)
            {
                SetPathDestination();
            }
        }


        // If the torca can't reach target, fail attacking
        if ((target.transform.position - (moveAgent.destination + attackOrigin.localPosition)).magnitude > comparison)
        {
            if (GetComponentInParent<Creature>().LogDebugs)
                Debug.Log($"Attack failed, further than {comparison} away");

            ResetNavigation();

            failSource.Cancel();


            if (!(animator == null || animator.GetBool("Die") || finishAnimationTrigger == ""))
            {
                animator.SetTrigger(finishAnimationTrigger);

                int maxLoops = 100;
                // wait for previous animation to finish       
                while (!animator.GetCurrentAnimatorStateInfo(0).loop && !animator.GetNextAnimatorStateInfo(0).IsName("Idle") && maxLoops > 0)
                {
                    maxLoops--;
                    await Task.Delay(100);
                }
            }

            failed = true;
            return;
        }

        if (creatureDeactivated)
            return;

        ResetNavigation();

        if (target.TryGetComponent(out Creature targetCreature))
        {
            if (!targetCreature.enabled || targetCreature.AttackSuccess(moveAgent.transform.position))
            {
                if (target.TryGetComponent(out agent))
                {
                    agent.enabled = false;
                }

                SetPathDestination();

                moveAgent.SetDestination(targetTransform.position - transform.TransformDirection(attackOrigin.localPosition));
                Task lastMovecheck = CheckDistanceToDestination();

                while (!lastMovecheck.IsCompletedSuccessfully)
                {
                    await Task.Delay(500);
                }

                ResetNavigation();
            } else
            {
                if (GetComponentInParent<Creature>().LogDebugs)
                    Debug.Log("Attack failed");

                failSource.Cancel();


                if (!(animator == null || animator.GetBool("Die") || finishAnimationTrigger == ""))
                {
                    animator.SetTrigger(finishAnimationTrigger);

                    int maxLoops = 100;
                    // wait for previous animation to finish       
                    while (!animator.GetCurrentAnimatorStateInfo(0).loop && !animator.GetNextAnimatorStateInfo(0).IsName("Idle") && maxLoops > 0)
                    {
                        maxLoops--;
                        await Task.Delay(100);
                    }
                }

                failed = true;
                return;
            }
        }
        else if (target.TryGetComponent(out CC_PlayerController player))
        {
            player.GoDie();
        }
            
        base.DoAction();
    }
}
