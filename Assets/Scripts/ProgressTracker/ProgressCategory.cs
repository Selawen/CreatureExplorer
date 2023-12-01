using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressCategory : ProgressObject
{
    [field: ShowOnly(2)] [field: SerializeField] public float Percentage { get; protected set; }
    [field: Range(1, 100)] [field: SerializeField] public int TotalAmount { get; protected set; }
    [field: SerializeField] public int PlayerAmount { get; protected set; }

    [field: SerializeField] public ProgressCategory[] Tracked { get; private set; }
    [field: SerializeField] public ProgressObject[] TrackedOjects { get; private set; }

    public void Initialise()
    {
        if (TrackedOjects.Length > 0)
        {
            foreach (ProgressObject pObj in TrackedOjects)
            {
                pObj.Initialise(this);
            }
        }

        if (Tracked.Length < 1)
            return;

        TotalAmount = Tracked.Length + TrackedOjects.Length;

        foreach (ProgressCategory p in Tracked)
        {
            p.Initialise(this);
        }

        Update();
    }

    public override void Initialise(ProgressCategory parent)
    {
        base.Initialise(parent);

        if (Tracked.Length < 1)
        {
            PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
            Percentage = (float)PlayerAmount / TotalAmount;

            UpdateCompletion();
        } else
        {
            Initialise();
        }
    }

    public override bool IsID(string id, out ProgressObject result)
    {
        result = this;
        if (id == ID)
            return true;
        else if (Tracked.Length > 0)
        {
            foreach (ProgressCategory progress in Tracked)
            {
                if (IsID(id, out result))
                    return true;
            }
        }

        return false;
    }

    public override void AddProgress(int amount = 1)
    {
        PlayerAmount += 1;
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = Percentage = (float)PlayerAmount / TotalAmount;

        UpdateCompletion();
    }

    private void SetProgress(int amount)
    {
        PlayerAmount = amount;
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = Percentage = (float)PlayerAmount / TotalAmount;

        UpdateCompletion();
    }

    protected void UpdateCompletion()
    {
        if (Percentage >= 1)
        {
            Completed = true;
        }

        if (category != null)
        {
            category.Update();
        }

    }

    public void Update()
    {
        if (Completed || Tracked.Length<1)
            return;

        float averageProgress = 0;
        int completedCount = 0;

        foreach (ProgressCategory p in Tracked)
        {
            averageProgress += p.Percentage;

            completedCount += p.Completed ? 1 : 0;
        }

        averageProgress /= Tracked.Length;

        Percentage = averageProgress;

        SetProgress(completedCount);

        UpdateCompletion();
    }
}
