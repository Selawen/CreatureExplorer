using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateBehaviour : EvaluateProgress
{
    // TODO: make async
    public static bool Evaluate(PictureInfo pictureInfo)
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
                            UpdateTrackedProgress(rightProgress);
                    }
                }
            }
        }
        return false;
    }
}
