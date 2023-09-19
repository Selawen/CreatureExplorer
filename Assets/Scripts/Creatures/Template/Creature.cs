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
    [SerializeField] private WorldState currentWorldState;
    [field:SerializeField] private Goal[] possibleGoals;
    [field:SerializeField] private List<Action> currentPlan;
    
    private Goal currentGoal;
    private Behaviour currentAction;
    private GameObject currentActionBehaviour;
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
        currentGoal = possibleGoals[0];

        if (currentWorldState.WorldStates.Length <=0)
        {
            currentWorldState = new WorldState();

            // TODO: remove, only used for testing
            for (int x = 0; x < currentWorldState.WorldStates.Length; x++)
            {
                if (currentWorldState.WorldStates[x].StateName == StateType.isHungry)
                {
                    currentWorldState.WorldStates[x].SetValue(true);
                    break;
                }
            }
        }

        currentPlan = planner.Plan(currentGoal, currentWorldState);

        StartAction();

        if (debug)
        {
            goalText.text = currentGoal.Name;
        }
    }

    void Update()
    {
        // if an action has failed try and generate a new goal
        if (currentAction.failed)
        {
            GenerateNewGoal();
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
        currentActionBehaviour = Instantiate(currentPlan[0].ActionBehaviour.gameObject);
        currentAction = currentActionBehaviour.GetComponent<Behaviour>();

        currentTarget = currentAction.PerformAction(gameObject, currentTarget);

        if (debug)
        {
            actionText.text = currentPlan[0].Name;
        }
    }

    private void FinishAction()
    {
        foreach (StatePair effect in currentPlan[0].Effects.WorldStates)
        {
            if (debug)
            {
                Debug.Log("updating worldstate of " + effect.StateName.ToString());
            }

            for (int x = 0; x < currentWorldState.WorldStates.Length; x++)
            {
                if (currentWorldState.WorldStates[x].StateName == effect.StateName)
                {
                    currentWorldState.WorldStates[x].SetValue(effect.StateValue);
                    break;
                }
            }
        }

        // remove the action that has now finished from the plan
        currentPlan.RemoveAt(0);

        // check if goal has been reached
        foreach (StatePair goalTarget in currentGoal.Target.WorldStates)
        {
            bool reached = true;
            for (int x = 0; x < currentWorldState.WorldStates.Length; x++)
            {
                if (currentWorldState.WorldStates[x].StateName == goalTarget.StateName)
                {
                    reached = currentWorldState.WorldStates[x].StateValue == goalTarget.StateValue ? true:false;
                    break;
                }
            }

            if (!reached)
                break;
            else
            {
                GenerateNewGoal();
                return;
            }
        }

            StartAction();
    }

    private void GenerateNewGoal()
    {
        // TODO: add goal generation 
        if (debug)
        {
            goalText.text = "Goal reached";
            actionText.text = "";
        }

        this.enabled = false;
    }
}
