using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plan : ScriptableObject
{
    public bool PlanComplete { get; private set; }
    public float Cost { get; private set; }
    public float Reward { get; private set; }
    public float CostRewardRatio { get; private set; }
    public List<Action> ActionList { get; private set; }

    public  ActionKey[] currentActionPrerequisites { get; private set; }
    public  Condition planWorldState { get; private set; }

    public Plan()
    {
        Cost = 0;
        Reward = 0;
        ActionList = new List<Action>();
        PlanComplete = false;
    }

    public Plan(Plan p)
    {
        Cost = p.Cost;
        Reward = p.Reward;
        ActionList = p.ActionList;
        currentActionPrerequisites = p.currentActionPrerequisites;
        planWorldState = p.planWorldState;
        PlanComplete = ActionList[ActionList.Count - 1].RequirementsSatisfied(planWorldState);
    }

    public Plan(Action firstAction, Condition worldState)
    {
        ActionList = new List<Action>();
        AddAction(firstAction, true);
        planWorldState = worldState;
        PlanComplete = ActionList[ActionList.Count - 1].RequirementsSatisfied(planWorldState);
    }

    public void AddAction(Action action, bool firstAction = false)
    {
        Cost += action.Cost; 
        Reward += action.Reward;
        CostRewardRatio = Reward/ Cost;

        ActionList.Add(action);
        currentActionPrerequisites = action.Prerequisites;

        if (!firstAction)
        {
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

        PlanComplete = ActionList[ActionList.Count - 1].RequirementsSatisfied(planWorldState);    
    }

    public void MarkComplete()
    {
        PlanComplete = true;
    }
}
