using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "NewProgressTracker", menuName = "Game Settings/New Progress Tracker")]
public class Tracker : ScriptableObject
{
    [field: SerializeField] public Progress[] Progresses { get; private set; }
}
