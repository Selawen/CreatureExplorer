using UnityEngine;

public class Prey : Creature
{
    [Header("Prey")]
    [SerializeField] private CreatureState reactionToPredator;

    protected override void Start()
    {
        surroundCheck += CheckForPredators;
        base.Start();
    }

    protected override void UpdateCreatureState()
    {
        CheckForInterruptions(StateType.Tiredness, GetComponentInChildren<Flee>(), "Terrified", 90);
        base.UpdateCreatureState();
    }

    protected void ReactToThreat(Vector3 threatPosition)
    {
        WaryOff = threatPosition;
        SetConditionTrue(worldState, Condition.IsNearDanger);
    }

    protected void ReactToThreat(Vector3 threatPosition, CreatureState reaction)
    {
        UpdateValues(reaction);
        WaryOff = threatPosition;
        SetConditionTrue(worldState, Condition.IsNearDanger);
    }
    
    protected void ReactToThreat(Torca predator)
    {
        ReactToThreat(predator.transform.position, reactionToPredator);
    }

    private void CheckForPredators()
    {
        Torca predator = null;

        if (LookForObjects<Torca>.TryGetClosestObject(predator, transform.position, 40, out predator))
        {
            ReactToThreat(predator);
        }
    }

}
