using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTrashCan : PageComponentInteractor
{
    public override bool OnComponentDroppedOn(PageComponent component)
    {
        component.SetInteractor(null);
        Destroy(component.gameObject);

        return true;
    }
}
