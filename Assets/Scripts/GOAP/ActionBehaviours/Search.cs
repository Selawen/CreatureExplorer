using System.Threading.Tasks;
using UnityEngine;

public class Search : Action
{
    [SerializeField] private SearchTarget searchTarget;

    public override GameObject PerformAction(GameObject creature, GameObject target)
    {
        FailCheck(failToken);

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
            Task.Run(() => DoAction(), token);
            return nearest.gameObject;
        }

        return target;
    }

    public override void CalculateCostAndReward(CreatureState currentState, MoodState targetMood, float targetMoodPrio)
    {
        base.CalculateCostAndReward(currentState, targetMood, targetMoodPrio);
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
