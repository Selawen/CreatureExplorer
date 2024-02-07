using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ShowCondition", menuName = "Titan Quests/New Show Condition")]
public class ShowCondition : QuestCondition
{
    [Tooltip("Using 'any' as the requiredID will make any picture be accepted")]
    [SerializeField] private string requiredID;
    public override bool Evaluate(PictureInfo pictureInfo)
    {
        if(requiredID.ToLower() == "any")
        {
            return true;
        }

        foreach (QuestableObject questable in pictureInfo.PictureObjects)
        {
            if (questable.QuestObjectID.ToLower() == requiredID.ToLower())
            {
                return true;
            }
        }
        return false;
    }
}
