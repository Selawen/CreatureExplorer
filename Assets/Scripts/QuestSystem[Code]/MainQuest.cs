using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMainQuest", menuName = "Titan Quests/New Main Quest")]
public class MainQuest : Quest
{
    [field: SerializeField] public int requiredAmount { get; private set; }

    [SerializeField] private string[] pictureNames;
    public string QuestProgress {get => $"{completedCount}/{requiredAmount}";}

    private int completedCount;

    [Button("Initialise")]
    public bool Update;

    public void Initialise()
    {
        pictureNames = new string[requiredConditions.Length];
        completedCount = 0;

        requiredAmount = (requiredAmount == 0)? requiredConditions.Length : requiredAmount;
    }

    public bool HasBeenEvaluated(string pictureName)
    {
        foreach (string s in pictureNames)
        {
            if (s == pictureName)
                return true;
        }
        return false;
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
        return completedCount >= requiredAmount;
    }

}
