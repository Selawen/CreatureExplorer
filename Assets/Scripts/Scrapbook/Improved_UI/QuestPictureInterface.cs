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

    [SerializeField] private Image handInBackground, descriptionBackground;
    [SerializeField] private Sprite defaultBackground, incorrectBackground, correctBackground;

    [SerializeField] private Image handInFrame;
    [SerializeField] private Sprite defaultFrame, incorrectFrame, correctFrame;

    [SerializeField] private GameObject pictureSlot;

    private PagePicture slottedPicture;

    private void Awake()
    {
        StaticQuestHandler.OnQuestOpened += () => 
        {
            descriptionBackground.color = Color.white;
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

        StaticQuestHandler.OnPictureClicked += (PagePicture picture) => 
        {
            picture.SetStepBackParent();
            if (OnComponentDroppedOn(picture))
            {
                picture.SetInteractor(this);
            }
        };

        StaticQuestHandler.OnQuestClosed += () => 
        {
            descriptionBackground.color = new Color(1, 1, 1, 0);
            handInBackground.color = new Color(1, 1, 1, 0);
            handInBackground.sprite = defaultBackground;
            handInFrame.sprite = defaultFrame;
            feedbackText.gameObject.SetActive(false); 
            descriptionText.gameObject.SetActive(false); 
            pictureSlot.SetActive(false); 
            if(slottedPicture != null)
            {
                slottedPicture.OnRevert();
            }
        };

        pictureSlot.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }


    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture) || slottedPicture != null) 
            return false;

        PagePicture picture = component as PagePicture;
        SlotPicture(picture);
        return true;
    }

    public override void RemoveFromInteractor(PageComponent component)
    {
        component.transform.localScale = Vector3.one;
        slottedPicture = null;

        handInBackground.sprite = defaultBackground;
        handInFrame.sprite = defaultFrame;
        feedbackText.text = "Please hand in your photo here";
    }

    private void SlotPicture(PagePicture picture)
    {
        picture.transform.SetPositionAndRotation(pictureSlot.transform.position, Quaternion.identity);
        picture.transform.localScale = Vector3.one * 2;

        picture.transform.SetParent(pictureSlot.transform, true);

        slottedPicture = picture;

        StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);
    }

    private IEnumerator CompleteQuest()
    {
        handInBackground.sprite = correctBackground;
        handInFrame.sprite = correctFrame;
        feedbackText.text = "Ah! This is what I was looking for!";

        StaticQuestHandler.OnQuestInputDisabled?.Invoke();

        yield return new WaitForSeconds(questCompletionWaitTime);

        handInBackground.color = new Color(1, 1, 1, 0);

        Destroy(slottedPicture.gameObject);
        slottedPicture = null;

        handInBackground.sprite = defaultBackground;
        handInFrame.sprite = defaultFrame;

        pictureSlot.SetActive(false);

        StaticQuestHandler.OnQuestClosed?.Invoke();
    }

}
