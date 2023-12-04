using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateBehaviour : EvaluateProgress
{
    public static bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (QuestableObject questable in pictureInfo.PictureObjects)
        {
            if (questable.TryGetComponent(out Creature creature))
            {

                foreach (ProgressCategory progress in TrackedCategories)
                {
                    if (progress.IsID(questable.QuestObjectID, out ProgressObject rightCategory))
                    {
                        rightCategory = (ProgressCategory)rightCategory;
                        if (rightCategory.IsID(creature.CurrentAction.GetType().ToString(), out ProgressObject rightProgress))
                            UpdateTrackedProgress(rightProgress);
                    }
                }
            }
        }
        return false;
    }
}
