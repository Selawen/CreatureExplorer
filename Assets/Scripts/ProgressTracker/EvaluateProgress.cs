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
    // TODO: make async
    public static bool EvaluatePictureProgress(PictureInfo pictureInfo)
    {
        foreach (QuestableObject questable in pictureInfo.PictureObjects)
        {
            if (questable.TryGetComponent(out Creature creature))
            {

                foreach (ProgressCategory progress in TrackedCategories)
                {
                    if (progress.IsCategory(questable.QuestObjectID, out ProgressCategory rightCategory))
                    {
                        if (rightCategory.HasID(creature.CurrentAction.GetType().ToString(), out ProgressObject rightProgress))
                            ProgressUIHandler.UpdateTrackedProgress(rightProgress);
                    }
                }
            }
        }
        return false;
    }

    public static void UpdateTrackedProgress(string progressID)
    {
        foreach (ProgressCategory progress in trackedProgress.ProgressCategories)
        {
            if (progress.HasID(progressID, out ProgressObject rightProgress))
            {
                ProgressUIHandler.UpdateTrackedProgress(rightProgress);
                //rightProgress.AddProgress();
            }
        }
    }
}
