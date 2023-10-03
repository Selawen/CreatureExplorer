using System;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    [SerializeField] private AnimationCurve AnnoyancePriority, FearPriority, HungerPriority, TirednessPriority, HappinessPriority, BoredomPriority;

    [field: SerializeField] private Goal[] possibleGoals;
    [Tooltip("initialised automatically")]
    [field: SerializeField] private Action[] possibleActions;
    [field: SerializeField] private Action defaultAction;

    private void OnValidate()
    {
        possibleActions = GetComponentsInChildren<Action>();
    }

    public bool GeneratePlan(CreatureState currentState, Effect worldState, out Goal goal, out List<Action> plan)
    {

        do
        {
            goal = GenerateGoal(currentState);
        } while (Plan(goal, currentState, worldState, out plan));

        return true;
    }

    public Goal GenerateGoal(CreatureState currentState)
    {
        CreatureState planState = currentState;
        float highestPrio = 0;
        StateType prioMood = StateType.Fear;

        foreach (MoodState state in planState.CreatureStates)
        {
            // TODO: make less naive, and more generic

            switch (state.MoodType)
            {
                case StateType.Annoyance:
                    if (AnnoyancePriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = AnnoyancePriority.Evaluate(state.StateValue / 100);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Fear:
                    if (FearPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = FearPriority.Evaluate(state.StateValue / 100);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Hunger:
                    if (HungerPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = HungerPriority.Evaluate(state.StateValue / 100);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Tiredness:
                    if (TirednessPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = TirednessPriority.Evaluate(state.StateValue / 100);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Happiness:
                    if (HappinessPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = HappinessPriority.Evaluate(state.StateValue / 100);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Boredom:
                    if (BoredomPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = BoredomPriority.Evaluate(state.StateValue / 100);
                        prioMood = state.MoodType;
                    }
                    break;
            }
        }

        List<Goal> goalChoices = new List<Goal>();
        foreach (Goal g in possibleGoals)
        {
            foreach (MoodState mood in g.Target)
            {
                if (mood.MoodType == prioMood)
                    goalChoices.Add(g);
            }
        }

        // TODO: actually generate diffferent goal if prioritised mood doesn't have any goals, instead of randomly choosing one
        if (goalChoices.Count < 1)
        {
            Goal randomGoal = possibleGoals[UnityEngine.Random.Range(0, possibleGoals.Length)];
            return randomGoal;
        }

        return goalChoices[UnityEngine.Random.Range(0, goalChoices.Count)];
    }

    public bool Plan(Goal goal, CreatureState currentStats, Effect worldState, out List<Action> plan)
    {
        // TODO: figureout way for this to not be necessairy
        CreatureState planStats = new CreatureState();
        foreach (MoodState mood in planStats.CreatureStates)
        {
            mood.SetValue(currentStats.Find(mood.MoodType).StateValue);
        }

        Effect planWorldState = worldState;

        plan = new List<Action>();

        if (goal == null)
        {
            plan.Add(defaultAction);
            return false;
        }


        MoodState[] goalPrerequisites = goal.Target;
        ActionKey[] currentActionPrerequisites;
        List<Action> viableActions = new List<Action>();
        Action chosenAction;

        // Look for action that satisfies goal
        viableActions.Clear();

        foreach (Action a in possibleActions)
        {
            if (a.GoalEffects.SatisfiesRequirements(goalPrerequisites, planStats))
            {
                viableActions.Add(a);
            }
        }

        if (viableActions.Count > 1)
        {
            chosenAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            plan.Add(chosenAction);

            currentActionPrerequisites = chosenAction.Prerequisites;
        }
        else if (viableActions.Count < 1)
        {
            plan.Clear();
            plan.Add(defaultAction);
            return false;
        }
        else
        {
            plan.Add(viableActions[0]);

            currentActionPrerequisites = viableActions[0].Prerequisites;
        }

        int failsafe = 0;

        // Create path towards action that satisfies goal
        while (!plan[plan.Count-1].RequirementsSatisfied(planWorldState))
        {
            // make sure Unity doesn't crash
            failsafe++;
            if (failsafe > 60)
            {
                Debug.LogError("took too long to generate plan");
                return false;
            }

            viableActions.Clear();

            foreach (Action a in possibleActions)
            {
                if (a.SatisfiesRequirements(currentActionPrerequisites, planWorldState))
                {
                    viableActions.Add(a);

                }
            }

            // TODO: refactor
            if (viableActions.Count > 1)
            {
                chosenAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
                plan.Add(chosenAction);

                for (int x = 0; x < chosenAction.ActionEffects.Length; x++)
                {
                    // if the statevalue is true set corresponding worldstate bit to 1, if not set it to 0
                    if (chosenAction.ActionEffects[x].StateValue)
                    {
                        planWorldState |= chosenAction.ActionEffects[x].EffectType;
                    }
                    else
                    {
                        planWorldState &= ~chosenAction.ActionEffects[x].EffectType;
                    }
                }

                currentActionPrerequisites = chosenAction.Prerequisites;
            }
            else
            {
                try
                {
                    plan.Add(viableActions[0]);

                    for (int x = 0; x < viableActions[0].ActionEffects.Length; x++)
                    {
                        // if the statevalue is true set corresponding worldstate bit to 1, if not set it to 0
                        if (viableActions[0].ActionEffects[x].StateValue)
                        {
                            planWorldState |= viableActions[0].ActionEffects[x].EffectType;
                        }
                        else
                        {
                            planWorldState &= ~viableActions[0].ActionEffects[x].EffectType;
                        }
                    }

                    currentActionPrerequisites = viableActions[0].Prerequisites;
                } catch (Exception e)
                {
                    plan.Clear();
                    plan.Add(defaultAction);
                    return false;
                }
            }

        } 

        plan.Reverse();

        return true;
    }
}
