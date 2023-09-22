using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    [field: SerializeField] private Goal[] possibleGoals;
    [Tooltip("initialised at startup")]
    [field: SerializeField] private Action[] possibleActions;

    int testGoal = 1;

    private void Awake()
    {
        possibleActions = GetComponentsInChildren<Action>();
    }

    public Goal GenerateGoal(CreatureState currentState)
    {
        // TODO: generate goal instead of randomly set
        testGoal++;
        return possibleGoals[testGoal%2];
    }

    public List<Action> Plan(Goal goal, CreatureState currentState)
    {
        List<Action> plan = new List<Action>();

        StatePair[] currentPrerequisites = goal.Target;
        List<Action> viableActions = new List<Action>();
        Action chosenAction;

        int failsafe = 0;
        do
        {
            // make sure Unity doen's crash
            failsafe++;
            if (failsafe > 60)
            {
                Debug.LogError("took too long to generate plan");
                return plan;
            }

            viableActions.Clear();

            foreach (Action a in possibleActions)
            {
                // TODO: make this work generally, not just return a set path
                if (a.Effects.SatisfiesRequirements(currentPrerequisites))
                {
                    viableActions.Add(a);
                }
            }

            if (possibleActions.Length > 1)
            {
                chosenAction = viableActions[Random.Range(0, viableActions.Count)];
                plan.Add(chosenAction);

                currentPrerequisites = chosenAction.Prerequisites;
            }
            else
            {
                plan.Add(viableActions[0]);

                currentPrerequisites = viableActions[0].Prerequisites;
            }

        } while (!currentState.SatisfiesRequirements(currentPrerequisites));

        plan.Reverse();

        return plan;
    }
}
