using System.Threading.Tasks;
using UnityEngine;

public class Eat : Action
{
    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        DoAction(target);
        FailCheck(token);

        return target;
    }

    protected override async void DoAction(GameObject target = null)
    {
        await Task.Delay((int)actionDuration * 1000);

        if (!failed)
        {
            Destroy(target);

            finished = true;
            source.Cancel();
        }
    }
}
