using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPictureInterface : PageComponentInteractor
{
    [SerializeField] private Transform pictureSlot;

    private PagePicture slottedPicture;

    private void Awake()
    {
        StaticQuestHandler.OnQuestOpened += () => gameObject.SetActive(true);
        StaticQuestHandler.OnQuestCompleted += () => gameObject.SetActive(false);
        StaticQuestHandler.OnPictureClicked += SlotPicture;

        gameObject.SetActive(false);
    }

    public void EvaluatePicture()
    {
        if(slottedPicture != null)
        {
            // Evaluate the slottedPicture to the current quest statue;
            StaticQuestHandler.OnPictureDisplayed?.Invoke(slottedPicture);
        }
    }

    public void SlotPicture(PagePicture picture)
    {
        if (slottedPicture != null) 
            return;

        picture.transform.position = pictureSlot.position;
        picture.transform.rotation = Quaternion.identity;
        picture.transform.localScale = Vector3.one;

        picture.transform.SetParent(pictureSlot);
        
        slottedPicture = picture;
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
