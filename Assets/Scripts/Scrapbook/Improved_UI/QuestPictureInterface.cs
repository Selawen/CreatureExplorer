using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestPictureInterface : PageComponentInteractor
{
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text feedbackText;

    private PagePicture slottedPicture;

    private void Awake()
    {
        StaticQuestHandler.OnQuestOpened = () => 
        { 
            gameObject.SetActive(true); 
            feedbackText.text = "Please hand in your photo here";
            descriptionText.text = StaticQuestHandler.CurrentQuestStatue.TitanQuest.QuestDescription;
        };

        StaticQuestHandler.OnQuestCompleted += () => gameObject.SetActive(false);

        StaticQuestHandler.OnQuestFailed += () => feedbackText.text = "Hmm, I don't believe this is what I'm looking for...";

        StaticQuestHandler.OnPictureClicked += SlotPicture;

        gameObject.SetActive(false);
    }

    //public void EvaluatePicture()
    //{
    //    if(slottedPicture != null)
    //    {
    //        // Evaluate the slottedPicture to the current quest statue;
    //        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);
    //    }
    //}

    public void SlotPicture(PagePicture picture)
    {
        if (slottedPicture != null) 
            return;

        picture.transform.position = transform.position;
        picture.transform.rotation = Quaternion.identity;
        picture.transform.localScale = Vector3.one;

        picture.transform.SetParent(transform);
        
        slottedPicture = picture;

        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);
    }

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture) || slottedPicture != null) 
            return false;

        SlotPicture(component as PagePicture);

        return true;
    }

    public override void RemoveFromInteractor(PageComponent component)
    {
        slottedPicture = null;
    }

}
