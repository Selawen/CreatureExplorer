using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ShowCondition", menuName = "Titan Quests/New Show Condition")]
public class ShowCondition : QuestCondition
{
    [Obsolete("Show condition will no longer use a direct reference, reference objects using required ID")]
    [Tooltip("Required object won't be supported after 22 December 2023, please use the required ID after this date")]
    [SerializeField] private QuestableObject requiredObject;

    [SerializeField] private string requiredID;
    public override bool Evaluate(PictureInfo pictureInfo)
    {
        if(requiredID.ToLower() == "any")
        {
            return true;
        }

        foreach (QuestableObject questable in pictureInfo.PictureObjects)
        {
            if (questable.QuestObjectID.ToLower() == requiredObject.QuestObjectID.ToLower() || questable.QuestObjectID.ToLower() == requiredID.ToLower())
            {
                return true;
            }
        }
        return false;
    }
}
