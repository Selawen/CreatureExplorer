using System.Collections;
using UnityEngine;

public class Eat : Action
{
    public override GameObject PerformAction(GameObject creature, GameObject target)
    { 
        StartCoroutine(CheckFinish());
        Destroy(target, actionDuration);

        return target;
    }

    protected override IEnumerator CheckFinish()
    {
        yield return new WaitForSeconds(actionDuration);
        finished = true;
        yield return null;
    }
}
