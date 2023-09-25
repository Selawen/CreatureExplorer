using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search : Action
{
    [SerializeField] private SearchTarget searchTarget;


    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        foreach (Collider c in Physics.OverlapSphere(creature.transform.position, 100))
        {
            switch (searchTarget)
            {
                case (SearchTarget.Food) :
                    
                    if (c.gameObject.TryGetComponent<Food>(out Food f))
                    {
                        StartCoroutine(CheckFinish());
                        return c.gameObject;
                    }
                    break;

                case (SearchTarget.Tree) :
                    
                    if (c.gameObject.TryGetComponent<Tree>(out Tree t))
                    {
                        StartCoroutine(CheckFinish());
                        return c.gameObject;
                    }
                    break;
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
    }
    public enum SearchTarget
    {
        Food,
        Tree
    }
}
