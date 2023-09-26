using System.Collections;
using System.Collections.Generic;
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

    private float timer = 0;

    /// <summary>
    /// called to perform the behaviour associated witn an action
    /// </summary>
    /// <param name="creature">the creature that is performing the action</param>
    /// <param name="target">the target of the action</param>
    /// <returns>returns a new target if the behaviour changes the target. Null if not</returns>
    abstract public GameObject PerformAction(GameObject creature, GameObject target);

    public void Update()
    {
        //cut action off after 1 minute
        timer += Time.deltaTime;
        if (timer > 60)
        {
            failed = true;
        }
    }
    public virtual void Reset()
    {
        finished = false;
        failed = false;

        timer = 0;
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

    abstract protected IEnumerator CheckFinish();

}
