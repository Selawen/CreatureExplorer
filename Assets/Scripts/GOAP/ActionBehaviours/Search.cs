using System.Threading.Tasks;
using UnityEngine;

public class Search : Action
{
    [SerializeField] private SearchTarget searchTarget;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        FailCheck(token);
        foreach (Collider c in Physics.OverlapSphere(creature.transform.position, 100))
        {
            switch (searchTarget)
            {
                case (SearchTarget.Food) :
                    
                    if (c.gameObject.TryGetComponent<Food>(out Food f))
                    {
                        DoAction();
                        return c.gameObject;
                    }
                    break;

                case (SearchTarget.Tree) :
                    
                    if (c.gameObject.TryGetComponent<Tree>(out Tree t))
                    {
                        DoAction();
                        return c.gameObject;
                    }
                    break;
            }
        }
        return target;
    }

    protected override async void DoAction(GameObject target = null)
    {
        await Task.Delay((int)actionDuration * 1000);

        base.DoAction();
    }

    public enum SearchTarget
    {
        Food,
        Tree
    }
}
