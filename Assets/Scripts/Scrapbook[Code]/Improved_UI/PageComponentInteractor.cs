using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PageComponentInteractor : MonoBehaviour
{
    public abstract bool OnComponentDroppedOn(PageComponent component);
    public virtual void RemoveFromInteractor(PageComponent component) { }

}
