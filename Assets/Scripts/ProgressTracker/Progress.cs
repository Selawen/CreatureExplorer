using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Progress
{
    [field: SerializeField] public string Name { get; private set; }
    [field: ShowOnly] [field: SerializeField] public float Percentage { get => PlayerAmount/TotalAmount; private set => Percentage = PlayerAmount / TotalAmount; }
    [field:Range(1, 100)] [field: SerializeField] public int TotalAmount { get; private set; }
    [field: SerializeField] public int PlayerAmount { get; private set; }
}
