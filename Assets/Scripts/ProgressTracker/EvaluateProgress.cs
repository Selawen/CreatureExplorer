using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateProgress : MonoBehaviour
{
    [field:SerializeField] protected Tracker trackerReference;
    private static Tracker trackedProgress;

    protected static ProgressCategory[] TrackedCategories
    {
        get => trackedProgress.ProgressCategories;
    }

    private void Awake()
    {
        if (trackedProgress == null)
        {
            trackedProgress = trackerReference;
        }
    }

    public static void UpdateTrackedProgress(string progressID)
    {
        foreach (ProgressCategory progress in trackedProgress.ProgressCategories)
        {
            if (progress.IsID(progressID, out ProgressObject rightProgress))
            {
                rightProgress.AddProgress();
            }
        }
    }

    protected static void UpdateTrackedProgress(ProgressObject progress)
    {
        progress.AddProgress();
    }
}
