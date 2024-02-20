using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestAltar : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Interact";

    [field: SerializeField] public MainQuest AltarQuest { get; private set; }
    private string questPrompt { get => AltarQuest.QuestDescription;}

    //[Tooltip("To which ring does this statue belong? 0 = Top Ring")]
    //[SerializeField] private int ringIndex;

    [SerializeField] private Material debugSwapMaterial;
    [SerializeField] private UnityEvent onAltarCompleted;

    private bool questFinished = false;

    public void Interact()
    {
        if (questFinished) return;

        DialogueUI.ShowText(questPrompt);

        //Cursor.lockState = CursorLockMode.Confined;

        // Move this to the player and subscribe to the QuestHandler's event there.
        //input.SwitchCurrentActionMap("Scrapbook");

        //questInfoText.gameObject.SetActive(true);
        //questShowButton.gameObject.SetActive(true);

        //StaticQuestHandler.OnPictureDisplayed += ShowPicture;
        //StaticQuestHandler.CurrentQuestStatue = this;
        StaticQuestHandler.OnQuestClosed += () =>
        {
            PagePicture.OnPictureClicked -= StaticQuestHandler.OnPictureClicked.Invoke;
            //StaticQuestHandler.OnPictureDisplayed -= ShowPicture;
        };

        PagePicture.OnPictureClicked += StaticQuestHandler.OnPictureClicked.Invoke;

        StaticQuestHandler.OnQuestOpened?.Invoke();
    }

    public void Activate()
    {
        // TODO: add quest to quest tracker
    }

    public void ShowPicture(PagePicture picture)
    {
        // To do: Evaluate whether any of the objects in the picture info is the object that we're looking for/
        // Also check if there are additional conditions and evaluate these too.
        if (AltarQuest.EvaluateQuestStatus(picture.PictureInfo))
        {
            StaticQuestHandler.OnQuestCompleted?.Invoke();
            questFinished = true;

            // Will be removed when correct visual feedback is implemented
            InteractionPrompt = string.Empty;
            DebugChangeMaterialVisuals();

            // TODO: change to shrine
            GrandTemple.OnStatueCleared?.Invoke();

            Cursor.lockState = CursorLockMode.Locked;
            onAltarCompleted?.Invoke();

            PagePicture.OnPictureClicked = null;
            return;
        }
        StaticQuestHandler.OnQuestFailed?.Invoke();

    }

    // Will be removed when correct visual feedback is implemented
    public void DebugChangeMaterialVisuals()
    {
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = debugSwapMaterial;
        }
    }
}
