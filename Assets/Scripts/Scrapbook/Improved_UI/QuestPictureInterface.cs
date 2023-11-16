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
        StaticQuestHandler.OnQuestOpened = () => 
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

        StaticQuestHandler.OnQuestClosed += () => { feedbackText.gameObject.SetActive(false); descriptionText.gameObject.SetActive(false); };

        pictureSlot.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }

    //public void EvaluatePicture()
    //{
    //    if(slottedPicture != null)
    //    {
    //        // Evaluate the slottedPicture to the current quest statue;
    //        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);
    //    }
    //}

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture) || slottedPicture != null) 
            return false;

        Debug.Log("Passed the dropping check, now slotting picture in the UI!");
        SlotPicture(component as PagePicture);

        return true;
    }

    public override void RemoveFromInteractor(PageComponent component)
    {
        Debug.Log("Removing picture from slot");
        slottedPicture = null;
    }

    private void SlotPicture(PagePicture picture)
    {
        if (slottedPicture != null) 
            return;

        picture.transform.position = pictureSlot.transform.position;
        picture.transform.rotation = Quaternion.identity;
        picture.transform.localScale = Vector3.one;

        picture.transform.SetParent(pictureSlot.transform, true);

        picture.SetInteractor(this);
        slottedPicture = picture;

        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);
    }

}
