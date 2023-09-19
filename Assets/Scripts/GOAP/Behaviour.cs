using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Behaviour : MonoBehaviour
{
    public bool finished = false;
    public bool failed = false;

    protected float actionDuration = 2;

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
            DestroyImmediate(gameObject);
        }
    }

    abstract protected IEnumerator CheckFinish();
}
