using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TitanStatue : MonoBehaviour, IInteractable
{
    [field:SerializeField] public string InteractionPrompt { get; private set; } = "Interact";

    [field: SerializeField] public Quest TitanQuest { get; private set; }

    //[Tooltip("To which ring does this statue belong? 0 = Top Ring")]
    //[SerializeField] private int ringIndex;

    [SerializeField] private Material debugSwapMaterial;
    [SerializeField] private UnityEvent onQuestCompleted;

    private bool questFinished = false;

    public void Interact()
    {
        if (questFinished) return;

        Cursor.lockState = CursorLockMode.Confined;

        // Move this to the player and subscribe to the QuestHandler's event there.
        //input.SwitchCurrentActionMap("Scrapbook");

        //questInfoText.gameObject.SetActive(true);
        //questShowButton.gameObject.SetActive(true);

        StaticQuestHandler.OnPictureDisplayed += ShowPicture;
        StaticQuestHandler.CurrentQuestStatue = this;
        StaticQuestHandler.OnQuestClosed += () => 
        { 
            PagePicture.OnPictureClicked -= StaticQuestHandler.OnPictureClicked.Invoke;
            StaticQuestHandler.OnPictureDisplayed -= ShowPicture;
        };

        PagePicture.OnPictureClicked += StaticQuestHandler.OnPictureClicked.Invoke;

        StaticQuestHandler.OnQuestOpened?.Invoke();
    }
    public void ShowPicture(PagePicture picture)
    {
        // To do: Evaluate whether any of the objects in the picture info is the object that we're looking for/
        // Also check if there are additional conditions and evaluate these too.
        if (TitanQuest.EvaluateQuestStatus(picture.PictureInfo))
        {
            StaticQuestHandler.OnQuestCompleted?.Invoke();

            // Will be removed when correct visual feedback is implemented
            DebugChangeMaterialVisuals();
            GrandTemple.OnStatueCleared?.Invoke();

            Cursor.lockState = CursorLockMode.Locked;
            questFinished = true;
            InteractionPrompt = string.Empty;
            onQuestCompleted?.Invoke();

            PagePicture.OnPictureClicked = null;
            return;
        }
        StaticQuestHandler.OnQuestFailed?.Invoke();



    }

    // Will be removed when correct visual feedback is implemented
    public void DebugChangeMaterialVisuals()
    {
        foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = debugSwapMaterial;
        }
    }

        // Question: Does showing the picture consume it? => It shouldn't, it should stay slotted in the statue
    

}
