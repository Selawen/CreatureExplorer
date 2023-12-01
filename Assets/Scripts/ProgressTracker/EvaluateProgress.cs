using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateProgress : MonoBehaviour
{
    public static Tracker trackedProgress;

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
}
