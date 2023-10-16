using UnityEngine;

[CreateAssetMenu(fileName = "ShowCondition", menuName = "Titan Quests/New Show Condition")]
public class ShowCondition : QuestCondition
{
    [SerializeField] private QuestableObject requiredObject;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (QuestableObject questable in pictureInfo.PictureObjects)
        {
            if (questable.QuestObjectID == requiredObject.QuestObjectID)
            {
                return true;
            }
        }
        return false;
    }
}
