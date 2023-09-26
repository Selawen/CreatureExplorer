using System.Collections;
using UnityEngine;

public class Think : Action
{
    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        StartCoroutine(CheckFinish());
        return target;
    }

    protected override IEnumerator CheckFinish()
    {
        yield return new WaitForSeconds(actionDuration);
        finished = true;
        yield return null;
    }
}
