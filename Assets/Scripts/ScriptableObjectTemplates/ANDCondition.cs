using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ANDCondition", menuName = "Titan Quests/New AND Condition")]
public class ANDCondition : QuestCondition
{
    [SerializeField] QuestCondition[] conditions;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        if(conditions.Length < 2)
        {
            throw new System.NullReferenceException("AND Condition doesn't contain at least 2 conditions to evaluate! At least 2 are required to create AND behaviour");
        }
        foreach(QuestCondition condition in conditions)
        {
            if (!condition.Evaluate(pictureInfo))
            {
                return false;
            }
        }
        return true;
    }
}
