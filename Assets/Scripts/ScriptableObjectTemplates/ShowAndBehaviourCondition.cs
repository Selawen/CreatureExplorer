using UnityEngine;

[CreateAssetMenu(fileName = "ShowAndBehaviourCondition", menuName = "Titan Quests/New Show and Behaviour Condition")]
public class ShowAndBehaviourCondition : QuestCondition
{
    [SerializeField] private Creature requiredCreature;
    [SerializeField] private Action requiredBehaviour;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (IInteractable interactable in pictureInfo.PictureObjects)
        {
            if (interactable.GetType() == requiredCreature.GetType())
            {
                Creature c = interactable as Creature;
                // To do: Get the current action that the creature is doing and check if it is the required action;
                //if ()
                //{
                return true;
                //}
            }
        }
        return false;
    }
}
