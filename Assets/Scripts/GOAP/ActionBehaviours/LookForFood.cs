using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForFood : Behaviour
{
    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        foreach (Collider c in Physics.OverlapSphere(creature.transform.position, 100))
        {
            // TODO: maybe make a way to choose food more accurately instead of returning first one found
            if (c.gameObject.TryGetComponent<Food>(out Food f))
            {
                StartCoroutine(CheckFinish());
                return c.gameObject;
            }
        }

        failed = true;
        StartCoroutine(CheckFinish());

        return target;
    }

    protected override IEnumerator CheckFinish()
    {
        yield return new WaitForSeconds(actionDuration);
        finished = true;
        yield return null;

        DestroyImmediate(gameObject);
    }
}
