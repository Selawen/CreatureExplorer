using UnityEngine;

[CreateAssetMenu(fileName = "ShowAndBehaviourCondition", menuName = "Titan Quests/New Show and Behaviour Condition")]
public class ShowAndBehaviourCondition : QuestCondition
{
    [SerializeField] private string requiredCreatureClassName;
    [SerializeField] private Action requiredBehaviour;

    public override bool Evaluate(PictureInfo pictureInfo)
    {
        foreach (IInteractable interactable in pictureInfo.PictureObjects)
        {
            if (interactable.GetType().ToString().ToLower() == requiredCreatureClassName.ToLower())
            {
                Creature c = interactable as Creature;
                Debug.Log("This is the creature I was looking for, it's an: " + c.GetType());
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
