using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ORCondition", menuName = "Titan Quests/New OR Condition")]
public class ORCondition : QuestCondition
{
    [SerializeField] QuestCondition[] conditions;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        if (conditions.Length < 2)
        {
            throw new System.NullReferenceException("OR Condition doesn't contain at least 2 conditions to evaluate! At least 2 are required to create OR behaviour");
        }
        foreach (QuestCondition condition in conditions)
        {
            if (condition.Evaluate(pictureInfo))
            {
                return true;
            }
        }
        return false;
    }
}
