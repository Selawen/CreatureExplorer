using System.Threading.Tasks;
using UnityEngine;

public class Attack : Action
{
    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        DoAction(target);
        FailCheck(failToken);
        return target;
    }

    public override void CalculateCostAndReward(CreatureState currentState, MoodState targetMood, float targetMoodPrio)
    {
        base.CalculateCostAndReward(currentState, targetMood, targetMoodPrio);
    }

    protected override async void DoAction(GameObject target = null)
    {
        await Task.Delay((int)actionDuration * 1000);
        // TODO: implement attack

        base.DoAction();
    }
}
