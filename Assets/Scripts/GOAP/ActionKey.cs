using System;
using UnityEngine;

[Serializable]
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public class ActionKey
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
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

