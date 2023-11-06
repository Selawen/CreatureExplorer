using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTrashCan : PageComponentInteractor
{
    public override bool OnComponentDroppedOn(PageComponent component)
    {
        component.Remove();
        Destroy(component.gameObject);

        return true;
    }
}
