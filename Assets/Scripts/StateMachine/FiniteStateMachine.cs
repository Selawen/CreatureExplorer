using System.Collections.Generic;
using System;

public class FiniteStateMachine
{
    private Dictionary<Type, IState> allStates = new();
    private IState currentState;

    private List<StateTransition> allTransitions;
    private List<StateTransition> activeTransitions;

    public FiniteStateMachine(Type startState, params IState[] states)
    {
        allTransitions = new();

        foreach (IState state in states)
        {
            allStates.Add(state.GetType(), state);
        }

        SwitchState(startState);
    }
    public void OnUpdate()
    {
        currentState?.OnStateUpdate();
        foreach(StateTransition transition in activeTransitions)
        {
            if (transition.Condition())
            {
                SwitchState(transition.ToState.GetType());
            }
        }
    }
    public void OnFixedUpdate()
    {
        currentState?.OnStateFixedUpdate();
    }
    public void SwitchState(Type newState)
    {
        currentState?.OnStateExit();
        currentState = allStates[newState];
        activeTransitions = allTransitions.FindAll(transition => transition.FromState == currentState || transition.FromState == null);
        currentState?.OnStateEnter();
    }
    public void AddState(IState addedState)
    {
        allStates.Add(addedState.GetType(), addedState);
    }
    public void AddTransition(StateTransition transition)
    {
        allTransitions.Add(transition);
        activeTransitions = allTransitions.FindAll(transition => transition.FromState == currentState || transition.FromState == null);

    }
    public void AddTransition(Type fromState, Type toState, Func<bool> condition)
    {
        StateTransition newTransition = new StateTransition(allStates[fromState], allStates[toState], condition);
        allTransitions.Add(newTransition);
        activeTransitions = allTransitions.FindAll(transition => transition.FromState == currentState || transition.FromState == null);
    }

}
