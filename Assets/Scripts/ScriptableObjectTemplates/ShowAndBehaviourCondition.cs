using UnityEngine;

[CreateAssetMenu(fileName = "ShowAndBehaviourCondition", menuName = "Titan Quests/New Show and Behaviour Condition")]
public class ShowAndBehaviourCondition : QuestCondition
{
    [SerializeField] private string requiredCreatureClassName;
    [SerializeField] private Action requiredBehaviour;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (QuestableObject questable in pictureInfo.PictureObjects)
        {
            if (questable.TryGetComponent(out Creature creature))
            {
                if(creature.GetType().ToString().ToLower() == requiredCreatureClassName.ToLower())
                {
                    if(creature.CurrentAction.GetType() == requiredBehaviour.GetType())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
