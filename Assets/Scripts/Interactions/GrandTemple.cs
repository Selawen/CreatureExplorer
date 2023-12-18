using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandTemple : MonoBehaviour
{
    public static GrandTemple Instance { get; private set; }
    
    public static System.Action OnStatueCleared;
    public static System.Action OnRingExtended;

    [SerializeField] private Animator topRing, middleRing, bottomRing;
    [SerializeField] private Animator stairDisk;

    [SerializeField] private string[] firstStageDialogue, secondStageDialogue, thirdStageDialogue;

    [SerializeField] private int firstStageRequirement = 2, secondStageRequirement = 5, thirdStageRequirement = 9;

    private int statuesCleared;

    private void Awake()
    {
        Instance = this;
        OnStatueCleared += EvaluateStatue;
    }

    public void EvaluateStatue()
    {
        statuesCleared++;
        if(statuesCleared == firstStageRequirement)
        {
            OnRingExtended?.Invoke();
            topRing.SetTrigger("Extend");
            stairDisk.SetTrigger("Extend");

            DialogueUI.ShowText(firstStageDialogue);
        }
        if(statuesCleared == secondStageRequirement)
        {
            OnRingExtended?.Invoke();
            middleRing.SetTrigger("Extend");
            stairDisk.SetTrigger("Extend");

            DialogueUI.ShowText(secondStageDialogue);
        }
        if (statuesCleared == thirdStageRequirement)
        {
            OnRingExtended?.Invoke();
            bottomRing.SetTrigger("Extend");
            Destroy(stairDisk.gameObject);

            DialogueUI.ShowText(thirdStageDialogue);
        }
    }
}
