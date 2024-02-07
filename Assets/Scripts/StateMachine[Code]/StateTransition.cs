using System;

public class StateTransition
{
    public IState FromState;
    public IState ToState;
    public Func<bool> Condition;

    public StateTransition(IState fromState, IState toState, Func<bool> condition)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
    }

    public bool Evaluate()
    {
        return Condition();
    }
}
