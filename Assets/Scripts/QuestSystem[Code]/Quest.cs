using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Titan Quests/New Quest")]
public class Quest : ScriptableObject
{
     public string QuestDescription { get => questDescription; }

    [SerializeField, TextArea(2, 4)] private string questDescription;
    [SerializeField] protected QuestCondition[] requiredConditions { get; private set; }

    public virtual bool EvaluateQuestStatus(PictureInfo pictureInfo)
    {
        foreach(QuestCondition condition in requiredConditions)
        {
            if (!condition.Evaluate(pictureInfo))
            {
                return false;
            }
        }
        return true;
    }
}
