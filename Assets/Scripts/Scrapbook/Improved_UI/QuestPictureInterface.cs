using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPictureInterface : PageComponentInteractor
{
    private PagePicture slottedPicture;
    private TitanStatue currentQuestStatue;

    public void EvaluatePicture()
    {
        if(slottedPicture != null && currentQuestStatue != null)
        {
            // Evaluate the slottedPicture to the current quest statue;
            currentQuestStatue.ShowPicture(slottedPicture);
        }
    }

    public void SlotPicture(PagePicture picture)
    {
        if (slottedPicture != null) return;

        picture.transform.position = transform.position;
        picture.transform.rotation = Quaternion.identity;
        picture.transform.SetParent(transform);
        slottedPicture = picture;
    }

    public void DBG_LinkStatue(TitanStatue statue) => currentQuestStatue = statue;

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture) || slottedPicture != null) return false;

        PagePicture picture = component as PagePicture;
        SlotPicture(picture);

        return true;
    }

    public override void RemoveFromInteractor(PageComponent component)
    {
        slottedPicture = null;
    }

}
