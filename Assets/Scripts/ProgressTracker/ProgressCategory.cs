using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressCategory
{
    [field: SerializeField] public string Name { get; private set; }
    [field: ShowOnly(2)] [field: SerializeField] public float Percentage { get; protected set; }
    [field: Range(1, 100)] [field: SerializeField] public int TotalAmount { get; protected set; }
    [field: SerializeField] public int PlayerAmount { get; protected set; }
    [field: ShowOnly] [field: SerializeField] public bool Completed { get; protected set; }

    private ProgressCategory category;

    [field: SerializeField] public ProgressCategory[] Tracked { get; private set; }

    public void Initialise()
    {
        if (Tracked.Length < 1)
            return;

        TotalAmount = Tracked.Length;

        foreach (ProgressCategory p in Tracked)
        {
            p.Initialise(this);
        }

        Update();
    }

    public void Initialise(ProgressCategory parent)
    {
        category = parent;
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

    public void SetName(string name)
    {
        Name = name;
    }

    public void AddProgress(int amount = 1)
    {
        PlayerAmount += 1;
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = Percentage = (float)PlayerAmount / TotalAmount;

        UpdateCompletion();
    }

    public void SetProgress(int amount)
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
