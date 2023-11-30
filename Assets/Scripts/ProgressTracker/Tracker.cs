using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "NewProgressTracker", menuName = "Game Settings/New Progress Tracker")]
public class Tracker : ScriptableObject
{
    [field: SerializeField] public ProgressCategory[] ProgressCategories { get; private set; }

    [Button("UpdateProgress")]
    public bool update;

    public void UpdateProgress()
    {
        foreach (ProgressCategory category in ProgressCategories)
        {
            category.Initialise();
        }
    }
}
