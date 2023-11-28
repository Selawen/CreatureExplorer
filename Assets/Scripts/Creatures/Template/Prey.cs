using UnityEngine;

public class Prey : Creature
{
    [Header("Prey")]
    [SerializeField] private CreatureState reactionToPredator;
    [SerializeField] private float predatorAwarenessRange = 40;

    protected override void Start()
    {
        surroundCheck += CheckForPredators;
        base.Start();
    }

    protected override void UpdateCreatureState()
    {
        if (WaryOff == null || WaryOff == Vector3.zero|| waryLoudness == 0)
            return;
        bool nearDanger = (WaryOff - transform.position).sqrMagnitude < (waryLoudness + data.HearingSensitivity * CurrentAction.Awareness);
        worldState =  nearDanger? SetConditionTrue(worldState, Condition.IsNearDanger) : SetConditionFalse(worldState, Condition.IsNearDanger);
        CheckForInterruptions(StateType.Fear, GetComponentInChildren<Flee>(), "Terrified", 90);
        base.UpdateCreatureState();
    }

    protected void ReactToThreat(Vector3 threatPosition, float threatLoudness)
    {
        WaryOff = threatPosition;
        waryLoudness = threatLoudness;
    }

    protected void ReactToThreat(Vector3 threatPosition, CreatureState reaction, float threatLoudness)
    {
        UpdateValues(reaction);
        ReactToThreat(threatPosition, threatLoudness);
    }
    
    protected void ReactToThreat(Torca predator, float predatorLoudness) => ReactToThreat(predator.transform.position, reactionToPredator, predatorLoudness);

    private void CheckForPredators()
    {
        Torca predator = null;

        if (LookForObjects<Torca>.TryGetClosestObject(predator, transform.position, predatorAwarenessRange*CurrentAction.Awareness, out predator))
        {
            ReactToThreat(predator, 1);
        }
    }

}
