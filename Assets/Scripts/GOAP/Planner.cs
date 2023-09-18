using System;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    [field: SerializeField] private Action[] possibleActions;

    
    public Goal SetGoal(WorldState currentState)
    {
        return null;
    }

    public List<Action> Plan(Goal goal, WorldState currentState)
    {
        List<Action> plan = new List<Action>();

        foreach (Action a in possibleActions)
        {
            // TODO: make this work generally, not just return a set path
            plan.Add(a);
        }

        return plan;
    }
}
