using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "NewGoal", menuName = "GOAP/Create Goal")]
public class Goal: ScriptableObject
{
    [field:SerializeField] public string Name { get; private set; }

    [field: SerializeField] public WorldState Target { get; private set; }
}
