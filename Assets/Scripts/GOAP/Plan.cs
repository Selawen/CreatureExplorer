using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plan : ScriptableObject
{
    public bool PlanComplete { get; private set; }
    public float Cost { get; private set; }
    public float Reward { get; private set; }
    public List<Action> ActionList { get; private set; }

    public  ActionKey[] currentActionPrerequisites { get; private set; }
    public  Condition planWorldState { get; private set; }

    public Plan()
    {
        ActionList = new List<Action>();
        PlanComplete = false;
    }

    public Plan(Plan p)
    {
        ActionList = p.ActionList;
        currentActionPrerequisites = p.currentActionPrerequisites;
        planWorldState = p.planWorldState;
        PlanComplete = false;
    }

    public Plan(Action firstAction, Condition worldState)
    {
        ActionList = new List<Action>();
        ActionList.Add(firstAction);
        currentActionPrerequisites = firstAction.Prerequisites;
        planWorldState = worldState;
        PlanComplete = (ActionList[0].SatisfiesRequirements(currentActionPrerequisites, planWorldState));
    }

    public void AddAction(Action action)
    {
        ActionList.Add(action);
        currentActionPrerequisites = action.Prerequisites;

        for (int x = 0; x < action.ActionEffects.Length; x++)
        {
            // if the statevalue is true set corresponding worldstate bit to 1, if not set it to 0
            if (action.ActionEffects[x].StateValue)
            {
                planWorldState |= action.ActionEffects[x].EffectType;
            }
            else
            {
                planWorldState &= ~action.ActionEffects[x].EffectType;
            }
        }
    }

    public void MarkComplete()
    {
        PlanComplete = true;
    }
}
