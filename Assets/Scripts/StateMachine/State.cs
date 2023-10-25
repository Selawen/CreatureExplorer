using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour, IState
{
    public virtual void OnStateEnter() { }

    public virtual void OnStateExit() { }

    public virtual void OnStateFixedUpdate() { }

    public virtual void OnStateUpdate(){ }
}
