using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

abstract public class Action: MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Onomatopea { get; private set; }

    [field: SerializeField] private AudioClip sound;
    [field: SerializeField] private bool oneShot;

    // TODO: have cost be calculated based on situation?
    [field: SerializeField] public float Cost { get; protected set; }
    [field: SerializeField] public float BaseReward { get; private set; }
    [field: SerializeField] public float Reward { get; protected set; }

    [field: SerializeField] public CreatureState GoalEffects { get; private set; }
    [field: SerializeField] public ActionKey[] ActionEffects { get; private set; }
    [field: SerializeField] public ActionKey[] Prerequisites { get; private set; }


    public bool finished = false;
    public bool failed = false;

    [SerializeField] protected float actionDuration = 2;

    protected CancellationTokenSource failSource;
    protected CancellationToken failToken;
    protected CancellationTokenSource source;
    protected CancellationToken token;

    protected virtual void Awake()
    {
        failSource = new CancellationTokenSource();
        failToken = failSource.Token;
        source = new CancellationTokenSource();
        token = failSource.Token;
    }

    private void OnDisable()
    {
        failSource.Cancel();
        failSource.Dispose();
        source.Cancel();
        source.Dispose();
    }

    public GameObject ActivateAction(Creature creature, GameObject target)
    {
        if (GetComponentInParent<SoundPlayer>() != null)
        {
            GetComponentInParent<SoundPlayer>().PlaySound(sound, oneShot);
        }

        return PerformAction(creature, target);
    }

    /// <summary>
    /// called to perform the behaviour associated witn an action
    /// </summary>
    /// <param name="creature">the creature that is performing the action</param>
    /// <param name="target">the target of the action</param>
    /// <returns>returns a new target if the behaviour changes the target. Null if not</returns>
    public abstract GameObject PerformAction(Creature creature, GameObject target);

    public virtual void Reset()
    {
        failSource.Cancel();
        source.Cancel();

        failSource = new CancellationTokenSource();
        failToken = failSource.Token;
        source = new CancellationTokenSource();
        token = failSource.Token;

        finished = false;
        failed = false;
    }

    public virtual void CalculateCostAndReward(CreatureState currentState, MoodState targetMood, float targetMoodPrio)
    {
        Reward = BaseReward;
        foreach (MoodState mood in GoalEffects.CreatureStates)
        {
            // Add a bonus to the reward if this action influences the main mood in the right way
            if (mood.MoodType == targetMood.MoodType && (mood.Operator == StateOperant.Set || (targetMood.Operator == StateOperant.LessThan && mood.Operator == StateOperant.Subtract) || (targetMood.Operator == StateOperant.GreaterThan &&  mood.Operator == StateOperant.Add)))
            {
                //calculate reward bonus
                // TODO: balance so that creature is more likely to ruminate when not hungry and eat when hungrier (for example)
                Reward += (1-mood.StateValue*0.01f) ;
                Reward *= targetMoodPrio * mood.StateValue;
                //Debug.Log($"prio is {targetMoodPrio}, {Name} reward is upped from {BaseReward} to {Reward}");
            }
        }
    }

    /// <summary>
    /// does this action meet the requirements given
    /// </summary>
    /// <param name="requirements">the requirements to satisfy</param>
    /// <returns></returns>
    public bool SatisfiesRequirements(ActionKey[] requirements)
    {
        int targetsReached = 0;

        foreach (ActionKey target in requirements)
        {
            for (int x = 0; x < this.ActionEffects.Length; x++)
            {
                if (this.ActionEffects[x] == target)
                {
                    targetsReached++;
                    break;
                }
            }
        }

        return (targetsReached >= requirements.Length);
    }

    /// <summary>
    /// does this action meet the requirements given
    /// </summary>
    /// <param name="requirements">the requirements to satisfy</param>
    /// <param name="currentState">the condition before the action is performed</param>
    /// <returns></returns>
    public bool SatisfiesRequirements(ActionKey[] requirements, Condition currentState)
    {
        Condition actionEffect = currentState;
        int targetsReached = 0;

        for (int x = 0; x < this.ActionEffects.Length; x++)
        {
            // if the statevalue is true set corresponding worldstate bit to 1, if not set it to 0
            if (this.ActionEffects[x].StateValue)
            {
                actionEffect |= this.ActionEffects[x].EffectType;
            }
            else
            {
                actionEffect &= ~this.ActionEffects[x].EffectType;
            }
        }

        foreach (ActionKey target in requirements)
        {
            if ((target.StateValue && actionEffect.HasFlag(target.EffectType))||(!target.StateValue && !actionEffect.HasFlag(target.EffectType)))
            {
                targetsReached++;
                continue;
            }
        }

        return (targetsReached >= requirements.Length);
    }

    /// <summary>
    /// Are the requirements of this action satisfied by the Conditon given?
    /// </summary>
    /// <param name="currentState">the condition to check against</param>
    /// <returns></returns>
    public bool RequirementsSatisfied(Condition currentState)
    {
        int targetsReached = 0;

        foreach (ActionKey target in Prerequisites)
        {
            if ((target.StateValue && currentState.HasFlag(target.EffectType)) || (!target.StateValue && !currentState.HasFlag(target.EffectType)))
            {
                targetsReached++;
                continue;
            }
        }

        return (targetsReached >= Prerequisites.Length);
    }

    /// <summary>
    /// If the action takes more than 50 percent longer than it's supposed to, assume it has failed
    /// </summary>
    protected virtual async void FailCheck(CancellationToken cancelToken)
    {
        try
        {
            await Task.Delay((int)((actionDuration * 1.5f) * 1000), cancelToken);
            {
                failed = true;
                source.Cancel();
            }
        } catch (TaskCanceledException)
        {
            //Debug.Log($"{this.name} has finished");
        }
    }

    // TODO: make sure this action stops if playmode is stopped
    protected virtual async void DoAction(GameObject target = null)
    {
        // standard end of DoAction method, set to finished and cancel the failcheck if not failed already
        if (!failed)
        {
            finished = true;
            failSource.Cancel();
        }
    }
}
