using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Planner))]
public class Creature : MonoBehaviour
{
    [Header("Events")]
    // protected and public are swapped because header hates public fields
    [SerializeField] protected float hearingSensitivity = 1;
    public Vector3 WaryOff { get; protected set; }
    [Tooltip("The name of the script that is on this creature's foodsource")]
    [field: SerializeField] public string FoodSource { get; protected set; }

    [Header("Debugging")]
    [SerializeField] private bool showThoughts;
    [field: SerializeField] public bool logDebugs { get; private set; }
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;

    [Header("GOAP")]
    [SerializeField] protected Condition worldState;
    [SerializeField] private CreatureState currentCreatureState;
    [SerializeField] private CreatureState changesEverySecond;
    [SerializeField] private CreatureState reactionToAttack;
    [SerializeField] private CreatureState reactionToPlayer;
    [SerializeField] private List<Action> currentPlan;

    private Goal currentGoal;
    private Action currentAction;
    private GameObject currentTarget = null;

    private Planner planner;

    private bool sawPlayer = false;

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
        if (currentAction != null)
        {
            // if an action has failed try and generate a new goal
            if (currentAction.failed)
            {
                if (logDebugs)
                    Debug.Log("Action failed! " + currentAction.Name);

                GenerateNewGoal();
            }
            else if (currentAction.finished)
            {
                FinishAction();
            }
        } 
    }

#region GOAP
    private void StartAction()
    {
        currentAction = currentPlan[0];

        // reset values on action before running it
        currentAction?.Reset();

        currentTarget = currentAction.PerformAction(this, currentTarget);

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

    /// <summary>
    /// Update creatureState with effects set in changesEverySecond
    /// </summary>
    private void UpdateValues()
    {
        foreach (MoodState change in changesEverySecond.CreatureStates)
        {
            if (change.Operator == StateOperant.Add)
                currentCreatureState.AddValue(change.StateValue * Time.deltaTime, change.MoodType);
            else if (change.Operator == StateOperant.Subtract)
                currentCreatureState.AddValue(-change.StateValue * Time.deltaTime, change.MoodType);
        }

        UpdateCreatureState();
    }
    /// <summary>
    /// Update creatureState with effects of finished action
    /// </summary>
    /// <param name="updateWith">the CreatureState containing the Moodstates to update with</param>
    protected void UpdateValues(CreatureState updateWith)
    {        
        foreach (MoodState change in updateWith.CreatureStates)
        {
            if (change.Operator == StateOperant.Set)
                currentCreatureState.SetValue(change.StateValue, change.MoodType);
            else if (change.Operator == StateOperant.Add)
                currentCreatureState.AddValue(change.StateValue, change.MoodType);
            else if (change.Operator == StateOperant.Subtract)
                currentCreatureState.AddValue(-change.StateValue, change.MoodType);
        }

        UpdateCreatureState();
    }

    /// <summary>
    /// set the worldstate to reflect mood values
    /// </summary>
    private void UpdateCreatureState()
    {
        worldState = (currentCreatureState.Find(StateType.Hunger).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsHungry) : SetConditionFalse(worldState, Condition.IsHungry);
        worldState = (currentCreatureState.Find(StateType.Tiredness).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsSleepy) : SetConditionFalse(worldState, Condition.IsSleepy);
        worldState = (currentCreatureState.Find(StateType.Annoyance).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsAnnoyed) : SetConditionFalse(worldState, Condition.IsAnnoyed);
        worldState = (currentCreatureState.Find(StateType.Fear).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsFrightened) : SetConditionFalse(worldState, Condition.IsFrightened);
    }
    #endregion

    /// <summary>
    /// Is the attack on this creature successful?
    /// </summary>
    /// <returns></returns>
    public bool AttackSuccess(Vector3 attackSource)
    {
        // TODO: think about what to set the value to beat to
        ReactToAttack(attackSource);

        if (Random.Range(0, currentCreatureState.Find(StateType.Tiredness).StateValue) > 20)
        {
            currentAction.Reset();

            // TODO: implement proper reaction
            goalText.text = "DEAD";
            actionText.text = "";
            this.enabled = false;

            return true;
        } else 
            return false;
    }

    public void HearPlayer(Vector3 playerPos, float playerLoudness)
    {
        if ((transform.position - playerPos).sqrMagnitude < playerLoudness * hearingSensitivity)
            ReactToPlayer(playerPos);
        else if (sawPlayer)
        {
            ReactToPlayerLeaving(playerPos);
            //worldState = SetConditionFalse(worldState, Condition.IsNearDanger);
        }
    }

    protected virtual void ReactToAttack(Vector3 attackPos)
    {
        WaryOff = attackPos;
        UpdateValues(reactionToAttack);
        worldState = SetConditionTrue(worldState, Condition.IsNearDanger);

        if (logDebugs)
        {
            Debug.Log("Was Attacked");
        } 
    }

    protected virtual void ReactToPlayer(Vector3 playerPos)
    {
        sawPlayer = true;
        UpdateValues(reactionToPlayer);
        if (logDebugs)
        {
            Debug.Log("Noticed Player");
        } 
    }

    protected virtual void ReactToPlayerLeaving(Vector3 playerPos)
    {
        sawPlayer = false;
        if (logDebugs)
        {
            Debug.Log("Lost sight of Player");
        } 
    }

    private Condition SetConditionTrue(Condition currentState, Condition flagToSet)
    {
        return currentState |= flagToSet;
    }
    private Condition SetConditionFalse(Condition currentState, Condition flagToSet)
    {
        return currentState &= ~flagToSet;
    }
}
