using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    [SerializeField] private AnimationCurve AnnoyancePriority;
    [SerializeField] private AnimationCurve FearPriority;
    [SerializeField] private AnimationCurve HungerPriority;
    [SerializeField] private AnimationCurve TirednessPriority;

    [field: SerializeField] private Goal[] possibleGoals;
    [Tooltip("initialised at startup")]
    [field: SerializeField] private Action[] possibleActions;
    [field: SerializeField] private Action defaultAction;

    private void OnValidate()
    {
        possibleActions = GetComponentsInChildren<Action>();
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
                        highestPrio = AnnoyancePriority.Evaluate(state.StateValue);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Fear:
                    if (FearPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = FearPriority.Evaluate(state.StateValue);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Hunger:
                    if (HungerPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = HungerPriority.Evaluate(state.StateValue);
                        prioMood = state.MoodType;
                    }
                    break;
                case StateType.Tiredness:
                    if (TirednessPriority.Evaluate(state.StateValue/100) > highestPrio)
                    {
                        highestPrio = TirednessPriority.Evaluate(state.StateValue);
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
            Goal randomGoal = possibleGoals[Random.Range(0, possibleGoals.Length)];
            return randomGoal;
        }

        return goalChoices[Random.Range(0, goalChoices.Count)];
    }

    public List<Action> Plan(Goal goal, CreatureState currentState)
    {
        // TODO: figureout way for this to not be necessairy
        CreatureState planState = new CreatureState();
        foreach (MoodState mood in planState.CreatureStates)
        {
            mood.SetValue(currentState.Find(mood.MoodType).StateValue);
        }

        List<Action> plan = new List<Action>();

        if (goal == null)
        {
            plan.Add(defaultAction);
            return plan;
        }


        MoodState[] goalPrerequisites = goal.Target;
        ActionKey[] currentActionPrerequisites;
        List<Action> viableActions = new List<Action>();
        Action chosenAction;

        // Look for action that satisfies goal
        viableActions.Clear();

        foreach (Action a in possibleActions)
        {
            if (a.GoalEffects.SatisfiesRequirements(goalPrerequisites, planState))
            {
                viableActions.Add(a);
            }
        }

        if (viableActions.Count > 1)
        {
            chosenAction = viableActions[Random.Range(0, viableActions.Count)];
            plan.Add(chosenAction);

            currentActionPrerequisites = chosenAction.Prerequisites;
        }
        else if (viableActions.Count < 1)
        {
            plan.Add(defaultAction);
            currentActionPrerequisites = defaultAction.Prerequisites;
        }
        else
        {
            plan.Add(viableActions[0]);

            currentActionPrerequisites = viableActions[0].Prerequisites;
        }

        int failsafe = 0;
        // Create path towards action that satisfies goal
        while (currentActionPrerequisites.Length>0)
        {
            // make sure Unity doesn't crash
            failsafe++;
            if (failsafe > 60)
            {
                Debug.LogError("took too long to generate plan");
                return plan;
            }

            viableActions.Clear();

            foreach (Action a in possibleActions)
            {
                if (a.SatisfiesRequirements(currentActionPrerequisites))
                {
                    viableActions.Add(a);
                }
            }

            if (viableActions.Count > 1)
            {
                chosenAction = viableActions[Random.Range(0, viableActions.Count)];
                plan.Add(chosenAction);

                currentActionPrerequisites = chosenAction.Prerequisites;
            }
            else
            {
                plan.Add(viableActions[0]);

                currentActionPrerequisites = viableActions[0].Prerequisites;
            }

        } 

        plan.Reverse();

        return plan;
    }
}
