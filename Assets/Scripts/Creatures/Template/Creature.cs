using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Planner))]
public class Creature : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool debug;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;

    [Header("GOAP")]
    [SerializeField] private CreatureState currentCreatureState;
    [SerializeField] private CreatureState changesEverySecond;
    [SerializeField] private List<Action> currentPlan;
    
    private Goal currentGoal;
    private Action currentAction;
    private GameObject currentTarget = null;

    private Planner planner;

    private void Awake()
    {
        if (!debug)
        {
            goalText.transform.parent.gameObject.SetActive(false);
        }

        planner = GetComponent<Planner>();

        if (currentCreatureState.CreatureStates.Length <=0)
        {
            currentCreatureState = new CreatureState();
        }

        GenerateNewGoal();   
    }


    void FixedUpdate()
    {
        UpdateValues();

        // if an action has failed try and generate a new goal
        if (currentAction.failed)
        {
            if (debug)
                Debug.Log("Action failed! " + currentAction.Name);

            GenerateNewGoal();
        }
        else if (currentAction.finished)
        {
            FinishAction();
        }       
    }

    private void UpdateValues()
    {
        // Update creatureState with effects of finished action
        foreach (MoodState change in changesEverySecond.CreatureStates)
        {
            if (change.Operator == StateOperant.Add)
                currentCreatureState.AddValue(change.StateValue * Time.deltaTime, change.MoodType);
            else if (change.Operator == StateOperant.Subtract)
                currentCreatureState.AddValue(-change.StateValue * Time.deltaTime, change.MoodType);
        }
    }

    private void StartAction()
    {
        // reset values on previous action before starting next action
        currentAction?.Reset();

        currentAction = currentPlan[0];

        currentTarget = currentAction.PerformAction(gameObject, currentTarget);

        if (debug)
        {
            actionText.text = currentPlan[0].Name;
        }
    }

    private void FinishAction()
    {
        // Update creatureState with effects of finished action
        foreach (MoodState effect in currentAction.GoalEffects.CreatureStates)
        {
            if (effect.Operator == StateOperant.Set)
                currentCreatureState.SetValue(effect.StateValue, effect.MoodType);
            else if (effect.Operator == StateOperant.Add)
                currentCreatureState.AddValue(effect.StateValue, effect.MoodType);
            else if (effect.Operator == StateOperant.Subtract)
                currentCreatureState.AddValue(-effect.StateValue, effect.MoodType);
            
            if (debug)
            {
                Debug.Log("updated worldstate of " + effect.MoodType.ToString());
            }
        }

        // check if goal has been reached
        if (currentPlan.Count <= 1)
        {
            GenerateNewGoal();
            return;
        }
        
        // remove the action that has now finished from the plan
        currentPlan.RemoveAt(0);

        StartAction();
    }

    private void GenerateNewGoal()
    {
        currentGoal = planner.GenerateGoal(currentCreatureState);
        currentPlan = planner.Plan(currentGoal, currentCreatureState);

        currentTarget = null;

        StartAction();

        if (debug)
        {
            Debug.Log("Generated new goal: " + currentGoal);
            goalText.text = currentGoal.Name;
        }
    }
}
