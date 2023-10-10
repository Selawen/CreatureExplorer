using System;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    [SerializeField] private AnimationCurve AnnoyancePriority, FearPriority, HungerPriority, TirednessPriority, HappinessPriority, BoredomPriority;

    [field: SerializeField] private Goal[] possibleGoals;
    [Tooltip("initialised automatically")]
    [field: SerializeField] private Action[] possibleActions;
    [field: SerializeField] private Goal defaultGoal;
    [field: SerializeField] private Action defaultAction;

    private Dictionary<float, StateType> moodPriorities;
    private int prioIndex = 0;

    private void Awake()
    {
        moodPriorities = new Dictionary<float, StateType>();
    }

    private void OnValidate()
    {
        possibleActions = GetComponentsInChildren<Action>();
    }

    public bool GeneratePlan(CreatureState currentState, Condition worldState, out Goal goal, out List<Action> plan)
    {
        goal = defaultGoal;
        plan = new List<Action>();

        // try generating a plan from the highest priority action. if no plan viable, take next highest priority 
        prioIndex = 0;
        UpdatePriorities(currentState);
        do
        {
            if (prioIndex > 5)
            {
                goal = defaultGoal;
                plan.Clear();
                plan.Add(defaultAction);
                return false;
            }

            goal = GenerateGoal(currentState);
            prioIndex++;
        } while (!PlanActions(goal, currentState, worldState, out plan));

        if (plan.Count < 1)
        {
            Debug.LogError("no valid course of action found");
            goal = defaultGoal;
            plan.Add(defaultAction);
        }
        return true;
    }

    /// <summary>
    /// Update the priority values of each mood type
    /// </summary>
    /// <param name="currentState"></param>
    public void UpdatePriorities(CreatureState currentState)
    {
        CreatureState planState = currentState;

        moodPriorities.Clear();

        float prio = 0;
        foreach (MoodState state in planState.CreatureStates)
        {
            // TODO: make more generic, find better way to avoid duplicate keys
            switch (state.MoodType)
            {
                case StateType.Annoyance:
                    prio = AnnoyancePriority.Evaluate(state.StateValue / 100);

                    try
                    {
                        moodPriorities.Add(prio, StateType.Annoyance);
                    }
                    catch (Exception e)
                    {
                        moodPriorities.Add(Mathf.Clamp(prio + 0.0001f, 0, 1), StateType.Annoyance);
                    }

                    break;
                case StateType.Fear:
                    prio = FearPriority.Evaluate(state.StateValue / 100);

                    try
                    {
                        moodPriorities.Add(prio, StateType.Fear);
                    }
                    catch (Exception e)
                    {
                        moodPriorities.Add(Mathf.Clamp(prio + 0.0002f, 0, 1), StateType.Annoyance);
                    }

                    break;
                case StateType.Hunger:
                    prio = HungerPriority.Evaluate(state.StateValue / 100);
                    try
                    {
                        moodPriorities.Add(prio, StateType.Hunger);
                    }
                    catch (Exception e)
                    {
                        moodPriorities.Add(Mathf.Clamp(prio + 0.0003f, 0, 1), StateType.Annoyance);
                    }

                    break;
                case StateType.Tiredness:
                    prio = TirednessPriority.Evaluate(state.StateValue / 100);

                    try
                    {
                        moodPriorities.Add(prio, StateType.Tiredness);
                    }
                    catch (Exception e)
                    {
                        moodPriorities.Add(Mathf.Clamp(prio + 0.0004f, 0, 1), StateType.Annoyance);
                    }

                    break;
                case StateType.Happiness:
                    prio = HappinessPriority.Evaluate(state.StateValue / 100);

                    try
                    {
                        moodPriorities.Add(prio, StateType.Happiness);
                    }
                    catch (Exception e)
                    {
                        moodPriorities.Add(Mathf.Clamp(prio + 0.0005f, 0, 1), StateType.Annoyance);
                    }

                    break;
                case StateType.Boredom:
                    prio = BoredomPriority.Evaluate(state.StateValue / 100);
                    try
                    {
                        moodPriorities.Add(prio, StateType.Boredom);
                    }
                    catch (Exception e)
                    {
                        moodPriorities.Add(Mathf.Clamp(prio + 0.0006f, 0, 1), StateType.Annoyance);
                    }

                    break;
            }
        }
    }

    public Goal GenerateGoal(CreatureState currentState)
    {
        List<Goal> goalChoices = new List<Goal>();

        float[] prioValues = new float[moodPriorities.Count]; 
        moodPriorities.Keys.CopyTo(prioValues, 0);
        Array.Sort(prioValues);

        int goalPrioIndex = prioValues.Length-1-prioIndex;

        while (goalChoices.Count < 1 && goalPrioIndex >= 0)
        {
            foreach (Goal g in possibleGoals)
            {
                foreach (MoodState mood in g.Target)
                {
                    if (mood.MoodType == moodPriorities[prioValues[goalPrioIndex]])
                        goalChoices.Add(g);
                }
            }
            goalPrioIndex--;
        }

        // Return default goal if the priority doesn't yield a good one
        if (goalChoices.Count < 1)
        {
            return defaultGoal;
        }

        return goalChoices[UnityEngine.Random.Range(0, goalChoices.Count)];
    }

    public bool PlanActions(Goal goal, CreatureState currentStats, Condition worldState, out List<Action> plan)
    {
        plan = new List<Action>();

        if (goal == null)
        {
            plan.Add(defaultAction);
            return false;
        }

        List<Plan> possiblePlans = new List<Plan>();

        // TODO: figure out way for this to not be necessary
        CreatureState planStats = new CreatureState();
        foreach (MoodState mood in planStats.CreatureStates)
        {
            mood.SetValue(currentStats.Find(mood.MoodType).StateValue);
        }

        Condition planWorldState = worldState;
        
        MoodState[] goalPrerequisites = goal.Target;
        List<Action> viableActions = new List<Action>();

        // Look for action that satisfies goal
        viableActions.Clear();

        // TODO: take into account plan, not just top action
        foreach (Action a in possibleActions)
        {
            if (a.GoalEffects.SatisfiesRequirements(goalPrerequisites, planStats))
            {
                viableActions.Add(a);
            }
        }

        if (viableActions.Count < 1)
        {
            plan.Clear();
            plan.Add(defaultAction);
            return false;
        }

        for (int x = viableActions.Count-1; x >= 0; x--)
        {
            possiblePlans.Add(new Plan(viableActions[x], worldState));
        }

        // TODO: debugging
        int failsafe = 0;
        int completionCounter = 0;

        while (completionCounter < possiblePlans.Count)
        {
            completionCounter = 0;

            List<Plan> updatedPlans = new List<Plan>();
            List<Plan> extraPlans = new List<Plan>();

            foreach (Plan p in possiblePlans)
            {
                bool needNewPlan = false;
                if (p.PlanComplete)
                {
                    completionCounter++;
                    continue;
                }

                // make sure Unity doesn't crash
                failsafe++;
                if (failsafe > 150)
                {
                    Debug.LogError("took too long to generate plan");
                    plan.Clear();
                    plan.Add(defaultAction);
                    return false;
                }

                foreach (Action a in possibleActions)
                {
                    if (a.SatisfiesRequirements(p.currentActionPrerequisites, p.planWorldState))
                    {
                        if (!needNewPlan)
                        {
                            needNewPlan = true;
                            updatedPlans.Add(p);
                        }

                        Plan newPlan = new Plan(p);
                        extraPlans.Add(newPlan);
                        newPlan.AddAction(a);
                        if (a.RequirementsSatisfied(newPlan.planWorldState))
                        {
                            newPlan.MarkComplete();
                            completionCounter++;
                        }                        
                    }
                }

                // if no new course of action is found even though the plan isn't complete delete plan from list of possible plans
                if (!needNewPlan)
                {
                    updatedPlans.Add(p);
                }

            }

            foreach(Plan update in updatedPlans)
            {
                possiblePlans.Remove(update);
            }

            possiblePlans.AddRange(extraPlans);

            if (possiblePlans.Count < 1)
            {
                return false;
            }
        }

        float costRewardRatio = 0;
        foreach (Plan p in possiblePlans)
        {
            if ((p.Reward/p.Cost) > costRewardRatio)
            {
                costRewardRatio = (p.Reward / p.Cost);
                plan = possiblePlans[UnityEngine.Random.Range(0, possiblePlans.Count)].ActionList;
            }
        }
        plan.Reverse();

        return true;

        #region old code
        /*
        failsafe = 0;
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

        // TODO: return plan with highest cost/reward ratio
        plan.Reverse();

        return true;
        */
        #endregion
    }
}
