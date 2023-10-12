using UnityEngine;

[CreateAssetMenu(fileName = "ShowCondition", menuName = "Titan Quests/New Show Condition")]
public class ShowCondition : QuestCondition
{
    [SerializeField] private IInteractable requiredObject;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (IInteractable interactable in pictureInfo.PictureObjects)
        {
            if (interactable.GetType() == requiredObject.GetType())
            {
                return true;
            }
        }
        return false;
    }
}
