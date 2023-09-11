using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : IState
{
    protected bool isActive;
    public virtual void OnStateEnter() 
    {
        isActive = true;
    }

    public virtual void OnStateExit()
    {
        isActive = false;
    }

    public virtual void OnStateFixedUpdate() { }

    public virtual void OnStateUpdate(){ }
}
