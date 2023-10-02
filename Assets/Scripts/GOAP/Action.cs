using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

abstract public class Action: MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public CreatureState GoalEffects { get; private set; }
    [field: SerializeField] public ActionKey[] ActionEffects { get; private set; }
    [field: SerializeField] public ActionKey[] Prerequisites { get; private set; }


    public bool finished = false;
    public bool failed = false;

    [SerializeField] protected float actionDuration = 2;

    protected CancellationTokenSource source;
    protected CancellationToken token;

    private void Awake()
    {
        source = new CancellationTokenSource();
        token = source.Token;
    }

    private void OnDisable()
    {
        source.Cancel();
        source.Dispose();
    }

    /// <summary>
    /// called to perform the behaviour associated witn an action
    /// </summary>
    /// <param name="creature">the creature that is performing the action</param>
    /// <param name="target">the target of the action</param>
    /// <returns>returns a new target if the behaviour changes the target. Null if not</returns>
    abstract public GameObject PerformAction(GameObject creature, GameObject target);

    public virtual void Reset()
    {
        //source = new CancellationTokenSource();
        token = source.Token;

        finished = false;
        failed = false;
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
    /// If the action takes more than 50 percent longer than it's supposed to, assume it has failed
    /// </summary>
    protected virtual async void FailCheck(CancellationToken cancelToken)
    {
        try
        {
            await Task.Delay((int)((actionDuration * 1.5f) * 1000), cancelToken);
            failed = true;
        } catch (TaskCanceledException e)
        {
        }
    }

    // TODO: make sure this action stops if playmode is stopped
    protected virtual async void DoAction(GameObject target = null)
    {
        // standard end of DoAction method, set to finished and cancel the failcheck if not failed already
        if (!failed)
        {
            finished = true;
            source.Cancel();
        }
    }
}
