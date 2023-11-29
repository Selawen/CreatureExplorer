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

    protected ProgressCategory category;

    public void Initialise(ProgressCategory parent = null)
    {
        category = parent;

        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = PlayerAmount / TotalAmount;
    }

    public void AddProgress(int amount = 1)
    {
        PlayerAmount += 1;
        PlayerAmount = Mathf.Clamp(PlayerAmount, 0, TotalAmount);
        Percentage = Percentage = PlayerAmount / TotalAmount;

        if (category != null)
        {
            category.Update();
        }
    }
}
