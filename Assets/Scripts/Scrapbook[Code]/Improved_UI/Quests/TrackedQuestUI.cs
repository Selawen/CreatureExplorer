using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackedQuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questProgressText;
    // TODO: add slider?

    public void Setup(MainQuest quest)
    {
        questDescriptionText.text = quest.QuestDescription;
        questProgressText.text = quest.QuestProgress;
    }

    // Update is called once per frame
    public void UpdateQuest(string questProgress)
    {
        questProgressText.text = questProgress;
    }
}
