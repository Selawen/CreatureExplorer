using System.Threading.Tasks;
using UnityEngine;

public class Sleep : Action
{
    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        token = source.Token;
        DoAction();
        FailCheck(token);
        return target;
    }

    protected override async void DoAction( GameObject target = null)
    {
        await Task.Delay((int)actionDuration * 1000);

        base.DoAction();
    }
}
