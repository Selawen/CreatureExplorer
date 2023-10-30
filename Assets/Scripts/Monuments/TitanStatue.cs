using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TitanStatue : MonoBehaviour, IInteractable
{
    // To do: predetermine quests per instance of TitanStatue in the inspector
    [SerializeField] private QuestCondition condition;
    [SerializeField] private PlayerInput input;

    [SerializeField] private UnityEvent onQuestFinished;
    [SerializeField] private UnityEvent onWrongPicturePresented;

    [SerializeField] private Material debugSwapMaterial;

    private bool questFinished;


    // Possible quests:
    //      Show a picture of a specific object.
    //      Show a picture of a specific behaviour.
    //      Show a picture of a specific behaviour on a specific creature.

    public void Interact()
    {
        if (questFinished) return;

        Scrapbook.Instance.OpenPages();
        ScrapbookPanel.Instance.OpenPanel();
        Cursor.lockState = CursorLockMode.Confined;
        input.SwitchCurrentActionMap("Scrapbook");

        foreach(PagePicture picture in Scrapbook.Instance.GetCollectedPictures())
        {
            picture.OnPictureClicked -= picture.SelectForPlacement;
            picture.OnPictureClicked += () => { ShowPicture(picture); };
            //picture.OnPictureClicked = () => { ShowPicture(picture.PictureInfo); Debug.Log("Clicked on a picture that has to be shown now"); };
        }

        // To do: Open the picture collection and let the player pick a picture to show to the statue
        // Question: Do we want to be able to pick pictures that have been placed in the scrapbook?
        // Question: Does showing the picture consume it?
    }

    public void ShowPicture(PagePicture picture)
    {
        // To do: Evaluate whether any of the objects in the picture info is the object that we're looking for/
        // Also check if there are additional conditions and evaluate these too.
        if (condition.Evaluate(picture.PictureInfo))
        {
            Debug.Log("Something very cool happens");
            onQuestFinished?.Invoke();
            Destroy(picture.gameObject);
            questFinished = true;
        }
        else
        {
            onWrongPicturePresented?.Invoke();
            Debug.Log("That wasn't the droid, uh... answer I was looking for");
        }
    }

    public void DebugChangeMaterialVisuals()
    {
        foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = debugSwapMaterial;
        }
    }
}
