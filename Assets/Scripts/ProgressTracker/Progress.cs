using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Progress
{
    [field: SerializeField] public string Name { get; private set; }
    [field: ShowOnly(2)] [field: SerializeField] public float Percentage { get; protected set; }
    [field:Range(1, 100)] [field: SerializeField] public int TotalAmount { get; protected set; }
    [field: SerializeField] public int PlayerAmount { get; protected set; }
    [field: SerializeField] public bool completed { get; protected set; }

    private ProgressCategory category;

    public void Initialise(ProgressCategory parent = null)
    {
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = (float)PlayerAmount / TotalAmount;

        category = parent;

        UpdateCompletion();
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
            completed = true;
        }

        if (category != null)
        {
            category.Update();
        }

    }
}
