using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTrashCan : PageComponentInteractor
{
    [field: SerializeField] private AudioClip throwAwaySound;
    [field: SerializeField] private SoundPlayer soundPlayer;

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        component.SetInteractor(null);
        Destroy(component.gameObject);

        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(throwAwaySound, true);
        }

        return true;
    }
}
