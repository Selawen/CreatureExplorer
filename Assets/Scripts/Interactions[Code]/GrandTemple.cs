using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandTemple : MonoBehaviour
{
    public static GrandTemple Instance { get; private set; }
    
    public static System.Action OnStatueCleared;
    public static System.Action OnRingExtended;

    [SerializeField] private Animator templeAnimator;

    [SerializeField] private string[] firstStageDialogue, secondStageDialogue, thirdStageDialogue;

    [SerializeField] private int firstStageRequirement = 2, secondStageRequirement = 5, thirdStageRequirement = 9;

    private int statuesCleared;

    private void Awake()
    {
        Instance = this;
        if (templeAnimator == null)
            templeAnimator = GetComponentInChildren<Animator>();

        OnStatueCleared += EvaluateStatue;
    }

    public void EvaluateStatue()
    {
        statuesCleared++;
        if(statuesCleared == firstStageRequirement)
        {
            OnRingExtended?.Invoke();
            templeAnimator.SetTrigger("1stLayer");

            DialogueUI.ShowText(firstStageDialogue);
        }
        if(statuesCleared == secondStageRequirement)
        {
            OnRingExtended?.Invoke();
            templeAnimator.SetTrigger("2ndLayer");

            DialogueUI.ShowText(secondStageDialogue);
        }
        if (statuesCleared == thirdStageRequirement)
        {
            OnRingExtended?.Invoke();
            templeAnimator.SetTrigger("FullyComplete");

            DialogueUI.ShowText(thirdStageDialogue);
        }
    }
}
