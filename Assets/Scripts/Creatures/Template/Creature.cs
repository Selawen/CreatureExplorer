using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Planner))]
public class Creature : MonoBehaviour
{
    [Header("Creature Stats")]
    // protected and public are swapped because header hates public fields
    [SerializeField] protected float hearingSensitivity = 1;
    public Vector3 waryOff { get; protected set; }

    [Header("Debugging")]
    [SerializeField] private bool showThoughts;
    [SerializeField] private bool logDebugs;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;

    [Header("GOAP")]
    [SerializeField] protected Condition worldState;
    [SerializeField] private CreatureState currentCreatureState;
    [SerializeField] private CreatureState changesEverySecond;
    [SerializeField] private CreatureState reactionToPlayer;
    [SerializeField] private List<Action> currentPlan;

    private Goal currentGoal;
    private Action currentAction;
    private GameObject currentTarget = null;

    private Planner planner;

    private void Start()
    {
        if (!showThoughts)
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
            if (logDebugs)
                Debug.LogWarning("Action failed! " + currentAction.Name);

            GenerateNewGoal();
        }
        else if (currentAction.finished)
        {
            FinishAction();
        }       
    }
    private void StartAction()
    {
        currentAction = currentPlan[0];

        // reset values on action before running it
        currentAction?.Reset();

        currentTarget = currentAction.PerformAction(gameObject, currentTarget);

        if (showThoughts)
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
            
            if (logDebugs)
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
        if (!planner.GeneratePlan(currentCreatureState, worldState, out currentGoal, out currentPlan) && logDebugs)
        {
            Debug.Log("Failed in generating plan, resorting to deault action");
        }

        //currentGoal = planner.GenerateGoal(currentCreatureState);
        //
        //if (!planner.Plan(currentGoal, currentCreatureState, worldState, out currentPlan))
        //{
        //    currentGoal = defaultGoal;
        //}

        currentTarget = null;

        StartAction();

        if (logDebugs)
        {
            Debug.Log("Generated new goal: " + currentGoal);
        }

        if (showThoughts)
        {
            goalText.text = currentGoal.Name;
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

        UpdateCreatureState();
    }

    protected void UpdateValues(CreatureState updateWith)
    {
        // Update creatureState with effects of finished action
        foreach (MoodState change in updateWith.CreatureStates)
        {
            if (change.Operator == StateOperant.Set)
                currentCreatureState.SetValue(change.StateValue, change.MoodType);
            else if (change.Operator == StateOperant.Add)
                currentCreatureState.AddValue(change.StateValue * Time.deltaTime, change.MoodType);
            else if (change.Operator == StateOperant.Subtract)
                currentCreatureState.AddValue(-change.StateValue * Time.deltaTime, change.MoodType);
        }

        UpdateCreatureState();
    }

    private void UpdateCreatureState()
    {
        // TODO: refactor
        if (currentCreatureState.Find(StateType.Hunger).StateValue > 50)
        {
            worldState |= Condition.IsHungry;
        }
        else
        {
            worldState &= ~Condition.IsHungry;
        }

        if (currentCreatureState.Find(StateType.Tiredness).StateValue > 50)
        {
            worldState |= Condition.IsSleepy;
        }
        else
        {
            worldState &= ~Condition.IsSleepy;
        }

        if (currentCreatureState.Find(StateType.Annoyance).StateValue > 50)
        {
            worldState |= Condition.IsAnnoyed;
        }
        else
        {
            worldState &= ~Condition.IsAnnoyed;
        }

        if (currentCreatureState.Find(StateType.Fear).StateValue > 50)
        {
            worldState |= Condition.IsFrightened;
        }
        else
        {
            worldState &= ~Condition.IsFrightened;
        }
    }

    public void HearPlayer(Vector3 playerPos, float playerLoudness)
    {
        if ((transform.position - playerPos).sqrMagnitude < playerLoudness * hearingSensitivity)
            ReactToPlayer(playerPos);
        else
        {
            worldState &= ~Condition.IsNearDanger;
        }
    }

    protected virtual void ReactToPlayer(Vector3 playerPos)
    {
        UpdateValues(reactionToPlayer);
        if (logDebugs)
        {
            Debug.Log("Noticed Player");
        } 
    }

}
