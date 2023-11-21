using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestPictureInterface : PageComponentInteractor
{
    [SerializeField] private float questCompletionWaitTime = 3f;

    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text feedbackText;

    [SerializeField] private Image handInBackground;
    [SerializeField] private Sprite defaultBackground, incorrectBackground, correctBackground;

    [SerializeField] private Image handInFrame;
    [SerializeField] private Sprite defaultFrame, incorrectFrame, correctFrame;

    [SerializeField] private GameObject pictureSlot;

    private PagePicture slottedPicture;

    private void Awake()
    {
        StaticQuestHandler.OnQuestOpened += () => 
        {
            handInBackground.color = Color.white;
            pictureSlot.SetActive(true);
            
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "Please hand in your photo here";

            descriptionText.gameObject.SetActive(true);
            descriptionText.text = StaticQuestHandler.CurrentQuestStatue.TitanQuest.QuestDescription;
        };

        StaticQuestHandler.OnQuestCompleted += () => StartCoroutine(CompleteQuest());

        StaticQuestHandler.OnQuestFailed += () => 
        { 
            feedbackText.text = "Hmm, I don't believe this is what I'm looking for...";
            handInFrame.sprite = incorrectFrame;
            handInBackground.sprite = incorrectBackground;
        };

        StaticQuestHandler.OnPictureClicked += (PagePicture picture) => { OnComponentDroppedOn(picture); picture.SetInteractor(this); };

        StaticQuestHandler.OnQuestClosed += () => 
        {
            handInBackground.color = new Color(1, 1, 1, 0);
            handInBackground.sprite = defaultBackground;
            handInFrame.sprite = defaultFrame;
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
        component.transform.localScale = Vector3.one;
        slottedPicture = null;
    }

    private bool SlotPicture(PagePicture picture)
    {
        if (slottedPicture != null)
            return false;

        picture.transform.SetPositionAndRotation(pictureSlot.transform.position, Quaternion.identity);
        picture.transform.localScale = Vector3.one * 2;

        picture.transform.SetParent(pictureSlot.transform, true);

        slottedPicture = picture;

        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);

        return true;
    }

    private IEnumerator CompleteQuest()
    {
        handInBackground.sprite = correctBackground;
        handInFrame.sprite = correctFrame;
        feedbackText.text = "Ah! This is what I was looking for!";

        yield return new WaitForSeconds(questCompletionWaitTime);

        handInBackground.color = new Color(1, 1, 1, 0);
        pictureSlot.SetActive(false);

        StaticQuestHandler.OnQuestClosed?.Invoke();
    }

}