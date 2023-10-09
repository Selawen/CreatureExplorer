using System;
using UnityEngine;

[Serializable]
public class ActionKey
{
    [field: SerializeField] public Condition EffectType { get; private set; }
    [field: SerializeField] public bool StateValue { get; private set; }

    public ActionKey(Condition stateType, bool value)
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

[Flags]
public enum Condition
{
    IsHungry = 1 << 0,
    IsSleepy = 1 << 1,
    IsFrightened = 1 << 2,
    IsAnnoyed = 1 << 3,
    SeesFood = 1 << 4,
    IsNearFood = 1 << 5,
    SeesTree = 1 << 6,
    IsNearDanger = 1 << 7
}

