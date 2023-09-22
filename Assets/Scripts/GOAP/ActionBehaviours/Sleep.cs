using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : Action
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
