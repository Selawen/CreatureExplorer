using System.Threading.Tasks;
using UnityEngine;

public class Search : Action
{
    [SerializeField] private SearchTarget searchTarget;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        FailCheck(token);

        float distance = 1000;
        Collider nearest = null;

        foreach (Collider c in Physics.OverlapSphere(creature.transform.position, 40))
        {
            switch (searchTarget)
            {
                case (SearchTarget.Food):

                    if (c.gameObject.TryGetComponent<Food>(out Food f) && (c.transform.position - creature.transform.position).sqrMagnitude < distance)
                    {
                        distance = (c.transform.position - creature.transform.position).sqrMagnitude;
                        nearest = c;
                    }
                    break;

                case (SearchTarget.Tree):

                    if (c.gameObject.TryGetComponent<Tree>(out Tree t) && (c.transform.position - creature.transform.position).sqrMagnitude < distance)
                    {
                        distance = (c.transform.position - creature.transform.position).sqrMagnitude;
                        nearest = c;
                    }
                    break;
            }
        }

        if (nearest != null)
        {
            DoAction();
            return nearest.gameObject;
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
