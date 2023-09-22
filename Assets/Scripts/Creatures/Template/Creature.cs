using System;
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
    [SerializeField] private GameObject testTarget;

    [Header("GOAP")]
    [SerializeField] private CreatureState currentCreatureState;
    [field:SerializeField] private List<Action> currentPlan;
    
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
    }

    void Start()
    {
        planner = GetComponent<Planner>();

        if (currentCreatureState.CreatureStates.Length <=0)
        {
            currentCreatureState = new CreatureState();

            // TODO: remove, only used for testing
            for (int x = 0; x < currentCreatureState.CreatureStates.Length; x++)
            {
                if (currentCreatureState.CreatureStates[x].StateName == StateType.isHungry || currentCreatureState.CreatureStates[x].StateName == StateType.isSleepy)
                {
                    currentCreatureState.CreatureStates[x].SetValue(true);
                }
            }
        }

        currentGoal = planner.GenerateGoal(currentCreatureState);
        currentPlan = planner.Plan(currentGoal, currentCreatureState);

        StartAction();

        if (debug)
        {
            goalText.text = currentGoal.Name;
        }
    }

    void FixedUpdate()
    {
        // if an action has failed try and generate a new goal
        if (currentAction.failed)
        {
            currentGoal = planner.GenerateGoal(currentCreatureState);
            if (debug)
            {
                goalText.text = currentGoal.Name;
            }
        }
        else if (currentAction.finished)
        {
            FinishAction();
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
        foreach (StatePair effect in currentAction.Effects.CreatureStates)
        {
            for (int x = 0; x < currentCreatureState.CreatureStates.Length; x++)
            {
                if (currentCreatureState.CreatureStates[x].Equals(effect))
                {
                    currentCreatureState.CreatureStates[x].SetValue(effect.StateValue);
                    continue;
                }
            }

            if (debug)
            {
                Debug.Log("updated worldstate of " + effect.StateName.ToString());
            }
        }


        // check if goal has been reached
        if (currentCreatureState.SatisfiesRequirements(currentGoal.Target))
        {
            currentGoal = planner.GenerateGoal(currentCreatureState);
            currentPlan = planner.Plan(currentGoal, currentCreatureState);

            StartAction();

            if (debug)
            {
                Debug.Log("Generated new goal: " + currentGoal);
                goalText.text = currentGoal.Name;
            }

            return;
        }
        
        // remove the action that has now finished from the plan
        currentPlan.RemoveAt(0);

        StartAction();
    }
}
