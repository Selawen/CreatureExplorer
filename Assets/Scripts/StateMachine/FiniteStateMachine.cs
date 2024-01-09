using System.Collections.Generic;
using System;

public class FiniteStateMachine
{
    public IState CurrentState { get; private set; }

    private Dictionary<Type, IState> allStates = new();

    private List<StateTransition> allTransitions;
    private List<StateTransition> activeTransitions;

    private bool running;

    public FiniteStateMachine(Type startState, params IState[] states)
    {
        running = true;

        allTransitions = new();

        foreach (IState state in states)
        {
            state.InitializeState(this);
            allStates.Add(state.GetType(), state);
        }
        SwitchState(startState);
    }

    public void Pause()
    {
        running = false;
    }

    public void Resume()
    {
        running = true;
    }

    public void OnUpdate()
    {
        if (!running) return;

        CurrentState?.OnStateUpdate();
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
        CurrentState?.OnStateFixedUpdate();
    }

    public void SwitchState(Type newState)
    {
        if (!allStates.ContainsKey(newState))
            return;

        CurrentState?.OnStateExit();
        CurrentState = allStates[newState];
        activeTransitions = allTransitions.FindAll(transition => transition.FromState == CurrentState || transition.FromState == null);
        CurrentState?.OnStateEnter();
    }

    public void AddState(IState addedState)
    {
        allStates.Add(addedState.GetType(), addedState);
    }

    public void AddTransition(StateTransition transition)
    {
        allTransitions.Add(transition);
        activeTransitions = allTransitions.FindAll(transition => transition.FromState == CurrentState || transition.FromState == null);

    }

    public void AddTransition(Type fromState, Type toState, Func<bool> condition)
    {
        StateTransition newTransition = new StateTransition(allStates[fromState], allStates[toState], condition);
        allTransitions.Add(newTransition);
        activeTransitions = allTransitions.FindAll(transition => transition.FromState == CurrentState || transition.FromState == null);
    }

}
