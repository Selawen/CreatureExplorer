using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public bool Completed { get; protected set; }

    protected ProgressCategory category;

    public virtual void Initialise(ProgressCategory parent)
    {
        if (ID == "")
            ID = Name;

        category = parent;
    }

    public virtual void Reset()
    {
        Completed = false;
    }

    public virtual bool IsID(string id, out ProgressObject result)
    {
        result = this;
        if (id == ID)
            return true;

        return false;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public virtual void AddProgress(int amount = 1)
    {
        Completed = true;

        if (category != null)
        {
            category.Update();
        }
    }
}
