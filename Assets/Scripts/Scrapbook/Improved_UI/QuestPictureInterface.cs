using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestPictureInterface : PageComponentInteractor
{
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text feedbackText;

    [SerializeField] private GameObject pictureSlot;

    private PagePicture slottedPicture;

    private void Awake()
    {
        StaticQuestHandler.OnQuestOpened += () => 
        {
            pictureSlot.SetActive(true);
            
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "Please hand in your photo here";

            descriptionText.gameObject.SetActive(true);
            descriptionText.text = StaticQuestHandler.CurrentQuestStatue.TitanQuest.QuestDescription;
        };

        StaticQuestHandler.OnQuestCompleted += () => pictureSlot.SetActive(false);

        StaticQuestHandler.OnQuestFailed += () => feedbackText.text = "Hmm, I don't believe this is what I'm looking for...";

        StaticQuestHandler.OnPictureClicked += (PagePicture picture) => OnComponentDroppedOn(picture);

        StaticQuestHandler.OnQuestClosed += () => 
        { 
            feedbackText.gameObject.SetActive(false); 
            descriptionText.gameObject.SetActive(false); 
            pictureSlot.SetActive(false); 
        };

        pictureSlot.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }


    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture) || slottedPicture != null) 
            return false;

        return SlotPicture(component as PagePicture);
    }

    public override void RemoveFromInteractor(PageComponent component)
    {
        slottedPicture = null;
    }

    private bool SlotPicture(PagePicture picture)
    {
        if (slottedPicture != null)
            return false;

        picture.transform.SetPositionAndRotation(pictureSlot.transform.position, Quaternion.identity);
        picture.transform.localScale = Vector3.one;

        picture.transform.SetParent(pictureSlot.transform, true);

        picture.SetInteractor(this);
        slottedPicture = picture;

        Debug.Log("Slotted new picture in the quest interface");

        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);

        return true;
    }

}
