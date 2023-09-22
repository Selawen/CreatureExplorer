using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreatureState 
{
    [field: SerializeField] public StatePair[] CreatureStates { get; private set; }

    public CreatureState()
    {
        int stateAmounts = Enum.GetNames(typeof(StateType)).Length;

        CreatureStates = new StatePair[stateAmounts];

        for (int x = stateAmounts; x>0; )
        {
            x--;
            CreatureStates[x] = new StatePair((StateType)x, false);
        }
    }

    /// <summary>
    /// does this CreatureState meet the requirements given
    /// </summary>
    /// <param name="requirements">the requirements to satisfy</param>
    /// <returns></returns>
    public bool SatisfiesRequirements(StatePair[] requirements)
    {
        int targetsReached = 0;

        foreach (StatePair Target in requirements)
        {
            for (int x = 0; x < this.CreatureStates.Length; x++)
            {
                if (this.CreatureStates[x] == Target)
                {
                    targetsReached++;
                    break;
                }
            }

        }

        return (targetsReached >= requirements.Length);
    }
}

[Serializable]
public class StatePair
{
    [field: SerializeField] public StateType StateName { get; private set; }
    [field: SerializeField] public bool StateValue { get; private set; }

    public StatePair(StateType stateType, bool value)
    {
        StateName = stateType;
        StateValue = value;
    }

    public void SetValue(bool newValue)
    {
        StateValue = newValue;
    }

    /// <summary>
    /// Returns true if the state type is the same
    /// </summary>
    /// <param name="b">the statepair to compare the type of</param>
    /// <returns></returns>
    public bool Equals(StatePair b)
    {
        return (this.StateName == b.StateName);
    }
    
    public static bool operator ==(StatePair a, StatePair b)
    {
        return (a.StateName == b.StateName && a.StateValue == b.StateValue);
    }

    public static bool operator !=(StatePair a, StatePair b) => !(a == b);
}

public enum StateType
{
    isHungry, 
    isSleepy,
    isFrightened,
    isNearFood,
    seesFood
}
