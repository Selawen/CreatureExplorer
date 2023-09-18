using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldState 
{
    [field: SerializeField] public StatePair[] WorldStates { get; private set; }

    public WorldState()
    {
        int stateAmounts = Enum.GetNames(typeof(StateType)).Length;

        WorldStates = new StatePair[stateAmounts];

        for (int x = stateAmounts; x>0; )
        {
            x--;
            WorldStates[x] = new StatePair((StateType)x, false);
        }
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
    
}

public enum StateType
{
    isHungry, 
    isSleepy,
    isFrightened,
    isNearFood
}
