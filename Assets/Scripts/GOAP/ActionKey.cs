using System;
using UnityEngine;

[Serializable]
public class ActionKey
{
    [field: SerializeField] public Effect EffectType { get; private set; }
    [field: SerializeField] public bool StateValue { get; private set; }

    public ActionKey(Effect stateType, bool value)
    {
        EffectType = stateType;
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
    public bool Equals(ActionKey b)
    {
        return (this.EffectType == b.EffectType);
    }    
    
    public static bool operator ==(ActionKey a, ActionKey b)
    {
        return (a.EffectType == b.EffectType && a.StateValue == b.StateValue);
    }

    public static bool operator !=(ActionKey a, ActionKey b) => !(a == b);
}

public enum Effect
{
    IsHungry,
    IsSleepy,
    IsFrightened,
    IsAnnoyed,
    SeesFood,
    IsNearFood,
    SeesTree
}

