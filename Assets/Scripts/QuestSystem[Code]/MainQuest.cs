using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMainQuest", menuName = "Titan Quests/New Main Quest")]
public class MainQuest : Quest
{
    [ShowOnly] private string[] pictureNames;

    private int completedCount;

    [Button("Initialise")]
    public bool Update;

    private void Initialise()
    {
        pictureNames = new string[requiredConditions.Length];
        completedCount = 0;
    }

    public override bool EvaluateQuestStatus(PictureInfo pictureInfo)
    {
        int x = 0;
        List<QuestCondition> satisfiedConditions = new List<QuestCondition>();

        foreach (QuestCondition condition in requiredConditions)
        {
            if (satisfiedConditions.Contains(condition) || pictureNames[x]!="")
            {
                // don't evaluate duplicate quests on the same picture
            } 
            else if (condition.Evaluate(pictureInfo))
            {
                pictureNames[x] = pictureInfo.PicturePath;
                completedCount++;
                satisfiedConditions.Add(condition);
            }
            x++;
        }

        return completedCount >= pictureNames.Length;
    }

}
