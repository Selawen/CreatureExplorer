using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TitanStatue : MonoBehaviour, IInteractable
{
    [field:SerializeField] public string InteractionPrompt { get; private set; } = "Interact";

    // To do: predetermine quests per instance of TitanStatue in the inspector
    [SerializeField] private QuestCondition condition;
    [SerializeField] private PlayerInput input;

    [SerializeField] private UnityEvent onQuestFinished;
    [SerializeField] private UnityEvent onWrongPicturePresented;

    [SerializeField] private Material debugSwapMaterial;
    [SerializeField] private TMPro.TMP_Text questInfoText;
    [SerializeField] private UnityEngine.UI.Button questShowButton;
    [SerializeField] private QuestPictureInterface debugPictureInterface;
    //[SerializeField] private GameObject debugPictureInterfaceContainer;

    private bool questFinished = false;



    // Possible quests:
    //      Show a picture of a specific object.
    //      Show a picture of a specific behaviour.
    //      Show a picture of a specific behaviour on a specific creature.
    private void Awake()
    {
        //debugPictureInterfaceContainer.gameObject.SetActive(false);
        questInfoText.gameObject.SetActive(false);
        questShowButton.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (questFinished) return;

        questInfoText.text = condition.DebugDescription;
        questShowButton.onClick.RemoveAllListeners();
        questShowButton.onClick.AddListener(OpenQuest);

        Cursor.lockState = CursorLockMode.Confined;
        input.SwitchCurrentActionMap("Scrapbook");
        questInfoText.gameObject.SetActive(true);
        questShowButton.gameObject.SetActive(true);

        StaticQuestHandler.OnPictureDisplayed = ShowPicture;
    }
    public void ShowPicture(PagePicture picture)
    {
        // To do: Evaluate whether any of the objects in the picture info is the object that we're looking for/
        // Also check if there are additional conditions and evaluate these too.
        if (condition.Evaluate(picture.PictureInfo))
        {
            StaticQuestHandler.OnQuestCompleted?.Invoke();

            onQuestFinished?.Invoke();
            picture.Remove();

            //if (Scrapbook.Instance.GetCollectedPictures().Contains(picture))
            //{
            //    Scrapbook.Instance.RemovePictureFromCollection(picture);
            //}
            //Destroy(picture.gameObject);
            Cursor.lockState = CursorLockMode.Locked;
            questFinished = true;
        }
        else
        {
            StaticQuestHandler.OnQuestFailed?.Invoke();

            onWrongPicturePresented?.Invoke();
        }

        PagePicture.OnPictureClicked = null;

    }
    public void DebugChangeMaterialVisuals()
    {
        foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = debugSwapMaterial;
        }
    }

    private void OpenQuest()
    {
        questShowButton.gameObject.SetActive(false);
        questInfoText.gameObject.SetActive(false);
        PrepareScrapbookForPictureDisplaying();
    }

    private void PrepareScrapbookForPictureDisplaying()
    {
        Scrapbook.Instance.OpenPages();
        //debugPictureInterfaceContainer.SetActive(true);

        StaticQuestHandler.OnQuestOpened?.Invoke();

        //Cursor.lockState = CursorLockMode.Confined;
        //input.SwitchCurrentActionMap("Scrapbook");

        PagePicture.OnPictureClicked = StaticQuestHandler.OnPictureClicked.Invoke;

        // To do: Open the picture collection and let the player pick a picture to show to the statue
        // Question: Do we want to be able to pick pictures that have been placed in the scrapbook? => We do (currently in testing fase)
        // Question: Does showing the picture consume it? => It doesn't, but it stays slotted in the statue
    }

}
