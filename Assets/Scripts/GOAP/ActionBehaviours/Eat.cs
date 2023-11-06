using System.Threading.Tasks;
using UnityEngine;

public class Eat : Action
{
    public override GameObject PerformAction(Creature creature, GameObject target)
    {
        //Task.Run(() => DoAction(target), token);
        // destroy doen't play nice with async
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

        if (!token.IsCancellationRequested)
        {
            Destroy(target);

            base.DoAction();
        }
    }
}
