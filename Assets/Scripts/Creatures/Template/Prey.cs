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
        bool nearDanger = (WaryOff - transform.position).sqrMagnitude < (waryLoudness + hearingSensitivity);
        worldState =  nearDanger? SetConditionTrue(worldState, Condition.IsNearDanger) : SetConditionFalse(worldState, Condition.IsNearDanger);
        CheckForInterruptions(StateType.Fear, GetComponentInChildren<Flee>(), "Terrified", 90);
        base.UpdateCreatureState();
    }

    // TODO: factor in loudness
    protected void ReactToThreat(Vector3 threatPosition, float threatLoudness)
    {
        WaryOff = threatPosition;
        waryLoudness = threatLoudness;
        //worldState = SetConditionTrue(worldState, Condition.IsNearDanger);
    }

    protected void ReactToThreat(Vector3 threatPosition, CreatureState reaction, float threatLoudness)
    {
        UpdateValues(reaction);
        ReactToThreat(threatPosition, threatLoudness);
    }
    
    protected void ReactToThreat(Torca predator, float predatorLoudness)
    {
        ReactToThreat(predator.transform.position, reactionToPredator, predatorLoudness);
    }

    private void CheckForPredators()
    {
        Torca predator = null;

        if (LookForObjects<Torca>.TryGetClosestObject(predator, transform.position, 40, out predator))
        {
            ReactToThreat(predator, 1);
        }
    }

}
