using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Planner))]
public class Creature : MonoBehaviour
{
    [field:Header("Reaction To Surroundings")]
    [Tooltip("The name of the script that is on this creature's foodsource")]
    [field: SerializeField] public string FoodSource { get; protected set; }
    [Tooltip("The name of the script that is on this creature's sleeping spots")]
    [field: SerializeField] public string SleepSpot { get; protected set; }
    [field: SerializeField] protected float hearingSensitivity = 1;
    [SerializeField] protected float checkSurroundingsTimer = 0.5f;
    public Vector3 WaryOff { get; protected set; }
    protected float waryLoudness = 1;

    [SerializeField] protected float decayTimer = 10;

    [Header("Debugging")]
    [SerializeField] private bool showThoughts;
    [field: SerializeField] public bool LogDebugs { get; private set; }
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private TextMeshProUGUI soundText;

    [Header("GOAP")]
    [SerializeField] protected Condition worldState;
    [SerializeField] private CreatureState currentCreatureState;
    [SerializeField] private CreatureState changesEverySecond;
    [SerializeField] private CreatureState reactionToAttack;
    [SerializeField] private CreatureState reactionToPlayer;
    [SerializeField] private List<Action> currentPlan;
    public Action CurrentAction { get; private set; }

    protected delegate void CheckSurroundings();
    protected CheckSurroundings surroundCheck;

    private Goal currentGoal;
    private GameObject currentTarget = null;

    private Planner planner;

    private bool sawPlayer = false;

    protected virtual void Start()
    {
        if (!showThoughts)
        {
            goalText.gameObject.SetActive(false);
            actionText.gameObject.SetActive(false);
            soundText.gameObject.SetActive(true);
        }

        planner = GetComponent<Planner>();

        if (currentCreatureState.CreatureStates.Length <=0)
        {
            currentCreatureState = new CreatureState();
        }

        GenerateNewGoal();

        surroundCheck += CheckForFood;
        StartCoroutines();
    }

    protected virtual void FixedUpdate()
    {
        // TODO: make it possible to interrupt actions

        UpdateValues();
        if (CurrentAction != null)
        {
            // if an action has failed try and generate a new goal
            if (CurrentAction.failed)
            {
                DebugMessage("Action failed! " + CurrentAction.Name);

                GenerateNewGoal();
            }
            else if (CurrentAction.finished)
            {
                FinishAction();
            }
        } 
    }

    private void StartCoroutines()
    {
        StartCoroutine(LookAtSurroundings());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        CurrentAction.enabled = false;
        Destroy(gameObject, decayTimer);
    }

    private void OnEnable()
    {
        StartCoroutines();
        if (CurrentAction!= null)
            CurrentAction.enabled = true;
    }


    #region GOAP
    private void StartAction()
    {
        CurrentAction = currentPlan[0];

        // reset values on action before running it
        CurrentAction.Reset();

        currentTarget = CurrentAction.ActivateAction(this, currentTarget);

        if (showThoughts)
        {
            actionText.text = currentPlan[0].Name;
        } else
        {
            soundText.text = currentPlan[0].Onomatopea;
        }
    }

    private void FinishAction()
    {
        // Update creatureState with effects of finished action
        foreach (MoodState effect in CurrentAction.GoalEffects.CreatureStates)
        {
            if (effect.Operator == StateOperant.Set)
                currentCreatureState.SetValue(effect.StateValue, effect.MoodType);
            else if (effect.Operator == StateOperant.Add)
                currentCreatureState.AddValue(effect.StateValue, effect.MoodType);
            else if (effect.Operator == StateOperant.Subtract)
                currentCreatureState.AddValue(-effect.StateValue, effect.MoodType);
            
            if (LogDebugs)
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
        if (!planner.GeneratePlan(currentCreatureState, worldState, out currentGoal, out currentPlan) && LogDebugs)
        {
            Debug.Log("Failed in generating plan, resorting to deault action");
        }

        // reset values on last action before starting new plan
        if (CurrentAction != null)
            CurrentAction.Reset();

        currentTarget = null;

        StartAction();

        if (LogDebugs)
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
    /// Add given parameters to creaturestate 
    /// </summary>
    protected void UpdateValues(StateType changeState, float amount, StateOperant operant)
    {
        if (operant == StateOperant.Add)
            currentCreatureState.AddValue(amount * Time.deltaTime, changeState);
        else if (operant == StateOperant.Subtract)
            currentCreatureState.AddValue(-amount * Time.deltaTime, changeState);

        UpdateCreatureState();
    }

    /// <summary>
    /// Update creatureState with effects of finished action
    /// </summary>
    /// <param name="updateWith">the CreatureState containing the Moodstates to update with</param>
    public void UpdateValues(CreatureState updateWith)
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
    protected virtual void UpdateCreatureState()
    {
        CheckForInterruptions(StateType.Tiredness, GetComponentInChildren<Sleep>(), "Fell asleep");

        worldState = (currentCreatureState.Find(StateType.Hunger).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsHungry) : SetConditionFalse(worldState, Condition.IsHungry);
        worldState = (currentCreatureState.Find(StateType.Tiredness).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsSleepy) : SetConditionFalse(worldState, Condition.IsSleepy);
        worldState = (currentCreatureState.Find(StateType.Annoyance).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsAnnoyed) : SetConditionFalse(worldState, Condition.IsAnnoyed);
        worldState = (currentCreatureState.Find(StateType.Fear).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsFrightened) : SetConditionFalse(worldState, Condition.IsFrightened);
        worldState = (currentCreatureState.Find(StateType.Happiness).StateValue > 50) ? SetConditionTrue(worldState, Condition.IsHappy) : SetConditionFalse(worldState, Condition.IsHappy);
    }

    /// <summary>
    /// interrupt the current action if a creaturestate value is higher than the threshold
    /// </summary>
    /// <param name="interruptionSource"></param>
    /// <param name="threshold"></param>
    protected void CheckForInterruptions(StateType interruptionSource, Action associatedAction, string interruptionText = "", float threshold = 99)
    {
        // TODO: refactor
        // stop action to fall asleep if tiredness = 100
        if (currentCreatureState.Find(interruptionSource).StateValue >= threshold)
        {
            try
            {
                MoodState moodOperator = CurrentAction.GoalEffects.Find(interruptionSource);
                // if this action doen not impact tiredness or makes creature more tired
                if (moodOperator == null)
                {
                    Interrupt(associatedAction, interruptionText);
                }
                else if (moodOperator.Operator != StateOperant.Subtract)
                {
                    Interrupt(associatedAction, interruptionText);
                }
            } catch (NullReferenceException)
            {
                Interrupt(associatedAction, interruptionText);
            }
        }
    }

    protected void Interrupt(Action associatedAction, string debugText = "")
    {
        if (LogDebugs)
            Debug.Log($"interruption source: {associatedAction.Name}");
        
        CurrentAction.Reset();
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(transform.position);

        goalText.text = debugText;
        currentPlan.Clear();
        currentPlan.Add(associatedAction);
        StartAction();
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

        if (UnityEngine.Random.Range(0, currentCreatureState.Find(StateType.Tiredness).StateValue) > 20)
        {
            CurrentAction.Reset();

            // TODO: implement proper reaction
            goalText.text = "DEAD";
            soundText.text = "DEAD";
            actionText.text = "";
            this.enabled = false;

            return true;
        } else 
            return false;
    }

    protected virtual void ReactToAttack(Vector3 attackPos)
    {
        WaryOff = attackPos;
        UpdateValues(reactionToAttack);
        worldState = SetConditionTrue(worldState, Condition.IsNearDanger);

        DebugMessage("Was Attacked");
    }

    // TODO: factor in loudness
    public void HearPlayer(Vector3 playerPos, float playerLoudness)
    {
        if ((transform.position - playerPos).sqrMagnitude < playerLoudness * hearingSensitivity * CurrentAction.Awareness)
            ReactToPlayer(playerPos, playerLoudness);
        else if (sawPlayer)
        {
            ReactToPlayerLeaving(playerPos);
        }
    }

    protected virtual void ReactToPlayer(Vector3 playerPos, float playerLoudness)
    {
        sawPlayer = true;
        UpdateValues(reactionToPlayer);

        DebugMessage("Noticed Player");
    }

    protected virtual void ReactToPlayerLeaving(Vector3 playerPos)
    {
        sawPlayer = false;

        DebugMessage("Lost sight of Player");
    }

    protected IEnumerator LookAtSurroundings()
    {
        yield return new WaitForSeconds(checkSurroundingsTimer);
        StartCoroutine(LookAtSurroundings());
        surroundCheck.Invoke();
    }

    /// <summary>
    /// Checks for food in neighbourhood and ups the hunger value with the amount of food nearby
    /// </summary>
    protected void CheckForFood()
    {
        Food f = null;
        int foodcount = LookForObjects<Food>.CheckForObjects(f, transform.position, hearingSensitivity).Count;

        currentCreatureState.AddValue(foodcount, StateType.Hunger);

        //DebugMessage($"found {foodcount} {FoodSource}, hunger is now {currentCreatureState.Find(StateType.Hunger).StateValue}");
    }

    protected Condition SetConditionTrue(Condition currentState, Condition flagToSet)
    {
        return currentState |= flagToSet;
    }

    protected Condition SetConditionFalse(Condition currentState, Condition flagToSet)
    {
        return currentState &= ~flagToSet;
    }

    protected void DebugMessage(string message)
    {
        if (LogDebugs)
            Debug.Log(message);
    }
}
