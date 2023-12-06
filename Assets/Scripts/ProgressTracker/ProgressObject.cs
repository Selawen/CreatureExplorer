using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ProgressObject
{
    [field: FieldIndent(0.4f)] [field: SerializeField] public string Name { get; private set; }
    [field: FieldIndent(0.4f)] [field: SerializeField] public string ID { get; private set; }

    [field:Header("UI")]
    [field: FieldIndent(0.4f)] [field: SerializeField] public Sprite UnfinishedIcon { get; private set; }
    [field: FieldIndent(0.4f)] [field: SerializeField] public Sprite FinishedIcon { get; private set; }

    [field: Header("Tracking")]
    [field: FieldIndent(0.4f)] [field: SerializeField] public bool Completed { get; protected set; }

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

    public virtual bool HasID(string id, out ProgressObject result)
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
        if (Completed)
            return;

        Completed = true;

        if (category != null)
        {
            category.Update();
        }
    }
}
