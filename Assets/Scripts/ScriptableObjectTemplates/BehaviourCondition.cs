using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourCondition", menuName = "Titan Quests/New Behaviour Condition")]
public class BehaviourCondition : QuestCondition
{
    [SerializeField] private Action requiredBehaviour;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (IInteractable interactable in pictureInfo.PictureObjects)
        {
            if (interactable.GetType() == typeof(Creature))
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
