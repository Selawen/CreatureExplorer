using System.Threading.Tasks;
using UnityEngine;

public class Search : Action
{
    [SerializeField] private SearchTarget searchTarget;
    [SerializeField] private float searchRadius = 1000;

    public override GameObject PerformAction(Creature creature, GameObject target)
    {
        FailCheck(failToken);

        float distance = searchRadius;
        Collider nearest = null;

        foreach (Collider c in Physics.OverlapSphere(creature.transform.position, 40))
        {
            switch (searchTarget)
            {
                case (SearchTarget.Food):

                    if ((c.gameObject.GetComponent(creature.FoodSource) != null) && (c.transform.position - creature.transform.position).sqrMagnitude < distance)
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
