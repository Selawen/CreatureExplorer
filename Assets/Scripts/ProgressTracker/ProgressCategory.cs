using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressCategory : ProgressObject
{
    [field: ShowOnly(2, 0.4f)] [field: SerializeField] public float Percentage { get; protected set; }
    [field: FieldIndent(0.4f)] [field: SerializeField] public int TotalAmount { get; protected set; }
    [field: FieldIndent(0.4f)] [field: SerializeField] public int PlayerAmount { get; protected set; }

    [field: SerializeField] public ProgressCategory[] Tracked { get; private set; }
    [field: SerializeField] public ProgressObject[] TrackedObjects { get; private set; }

    protected int childCategoryCount { get => Tracked == null? 0 : Tracked.Length; }
    protected int childProgressCount { get => TrackedObjects == null? 0 : TrackedObjects.Length; }

    public void Initialise()
    {
        if (childProgressCount > 0)
        {
            foreach (ProgressObject pObj in TrackedObjects)
            {
                pObj.Initialise(this);
            }
        }

        TotalAmount = childCategoryCount + childProgressCount;

        if (childCategoryCount > 0)
        {
            foreach (ProgressCategory p in Tracked)
            {
                p.Initialise(this);
            }
        }

        Update();
    }

    public override void Initialise(ProgressCategory parent)
    {
        base.Initialise(parent);
        Initialise();
        /*
        if (Tracked.Length < 1)
        {
            TotalAmount = TrackedObjects.Length;
            PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
            Percentage = (float)PlayerAmount / TotalAmount;
            UpdateCompletion();
        }
        */
    }

    public override void Reset()
    {
        base.Reset();
        PlayerAmount = 0;
        Percentage = 0;

        if (childProgressCount > 0)
        {
            foreach (ProgressObject pObj in TrackedObjects)
            {
                pObj.Reset();
            }
        }

        foreach (ProgressCategory p in Tracked)
        {
            p.Reset();
        }

    }

    public bool IsCategory(string id, out ProgressCategory result)
    {
        result = this;
        id = id.ToLower();

        if (id == ID.ToLower())
            return true;
        else if (childCategoryCount > 0)
        {
            foreach (ProgressCategory progress in Tracked)
            {
                if (progress.IsCategory(id, out result))
                    return true;
            }
        }

        return false;
    }

    public override bool HasID(string id, out ProgressObject result)
    {
        result = this;

        if (id == ID)
            return true;
        else if (childCategoryCount > 0)
        {
            foreach (ProgressCategory progress in Tracked)
            {
                if (progress.HasID(id, out result))
                    return true;
            }
        }

        if (childProgressCount > 0)
        {
            foreach (ProgressObject progressObj in TrackedObjects)
            {
                if (progressObj.HasID(id, out result))
                    return true;
            }
        }

        return false;
    }

    public override void AddProgress(int amount = 1)
    {
        PlayerAmount += 1;
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage= (float)PlayerAmount / TotalAmount;

        UpdateCompletion();
    }

    public override float GetProgress()
    {
        return Percentage;
    }

    private void SetProgress(int amount)
    {
        PlayerAmount = amount;
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = (float)PlayerAmount / TotalAmount;

        UpdateCompletion();
    }

    protected void UpdateCompletion()
    {
        if (float.IsNaN(Percentage))
            Percentage = 0;
        else if (Percentage >= 1)
            Completed = true;

        if (category != null)
            category.Update();
    }

    public void Update()
    {
        if (Completed || (childCategoryCount < 1 && childProgressCount < 1))
        {
            UpdateCompletion();
            return;
        }

        float averageProgress = 0;
        int completedCount = 0;

        foreach (ProgressCategory p in Tracked)
        {
            averageProgress += p.Percentage;

            completedCount += p.Completed ? 1 : 0;
        }

        foreach (ProgressObject pO in TrackedObjects)
        {
            if (pO.Completed)
            {
                averageProgress++;
                completedCount ++;
            }
        }


        SetProgress(completedCount);
        averageProgress /= (childCategoryCount + childProgressCount);

        Percentage = float.IsNaN(averageProgress)? 0 : averageProgress ;

        UpdateCompletion();
    }
}
