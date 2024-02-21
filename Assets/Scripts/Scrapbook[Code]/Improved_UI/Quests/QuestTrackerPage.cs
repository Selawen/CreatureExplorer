using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrackerPage : MonoBehaviour
{
    [SerializeField] private Transform questUIGrid;
    [SerializeField] private GameObject questUIPrefab;

    private Dictionary<MainQuest, TrackedQuestUI> quests;

    // Start is called before the first frame update
    void Start()
    {
        quests = new Dictionary<MainQuest, TrackedQuestUI>();

        StaticQuestHandler.OnAltarActivated += AddQuest;
    }

    private void AddQuest(MainQuest altarQuest)
    {
        if (quests.Count == 0)
            StaticQuestHandler.OnAltarProgress += UpdateQuestProgress;

        TrackedQuestUI newQuestUI = Instantiate<GameObject>(questUIPrefab, questUIGrid).GetComponent<TrackedQuestUI>();
        
        newQuestUI.Setup(altarQuest);

        quests.Add(altarQuest, newQuestUI);
    }

    private void UpdateQuestProgress(MainQuest updatedQuest)
    {
        quests[updatedQuest].UpdateQuest(updatedQuest.QuestProgress);
    }
}
