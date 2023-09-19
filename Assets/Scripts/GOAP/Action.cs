using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "NewAction", menuName = "GOAP/Create Action")]
public class Action: ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public WorldState Effects { get; private set; }
    [field: SerializeField] public WorldState Prerequisites { get; private set; }
    [field: SerializeField] public Behaviour ActionBehaviour { get; private set; }
}
