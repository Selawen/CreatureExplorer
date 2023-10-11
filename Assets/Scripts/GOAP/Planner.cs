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
    private float currentPrioKey;

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
                Debug.LogError("no valid course of action found");
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
            plan.Clear();
            plan.Add(defaultAction);
            return false;
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
        currentPrioKey = prioValues[goalPrioIndex];

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

        // TODO: look at the impact of total plans on goal target?
        // Look for actions that satisfy goal target
        viableActions.Clear();

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
            //moodPriorities[currentPrioKey]
            viableActions[x].CalculateCostAndReward(currentStats, goal.Target[0], currentPrioKey);
            possiblePlans.Add(new Plan(viableActions[x], worldState));
        }

        int failsafe = 0;
        int completionCounter = 0;

        // While there are incomplete plans that have the potential to be completed, expand the incomplete plans
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

                        //moodPriorities[currentPrioKey]
                        MoodState mainMood = goal.Target[0];
                        foreach (MoodState mood in goal.Target)
                        {
                            if (mood.MoodType == moodPriorities[currentPrioKey])
                                mainMood = mood;
                        }

                        a.CalculateCostAndReward(currentStats, mainMood, currentPrioKey);
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

            // Remove the plans that have been updated, then add the updated versions to possibleplans
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

        float bestCostRewardRatio = 0;
        foreach (Plan p in possiblePlans)
        {
            Debug.Log($"reward/cost of plan ending with {p.ActionList[0]} is {p.CostRewardRatio}");
            if ((p.CostRewardRatio) > bestCostRewardRatio)
            {
                bestCostRewardRatio = p.CostRewardRatio;
                plan = p.ActionList; 
            }
        }
        plan.Reverse();

        return true;
    }
}
