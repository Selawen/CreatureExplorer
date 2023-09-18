using System;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    [field: SerializeField] private Action[] possibleActions;

    
    public List<Action> Plan(Goal goal, WorldState currentState)
    {
        return null;
    }
}
