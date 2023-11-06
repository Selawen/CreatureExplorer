using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapbookPage : PageComponentInteractor
{
    [SerializeField] private TMPro.TMP_Text pageNumberText;
    [SerializeField] private RectTransform defaultLayer;
    [SerializeField] private RectTransform overlayLayer;

    // This List is deprecated and will be removed after full implementation of the new UI system
    //private List<MoveablePageComponent> pageComponents = new();

    private List<PageComponent> newPageComponents = new();

    public void SetPageNumber(int pageNumber) => pageNumberText.text = pageNumber.ToString();

    //public List<PageComponent> GetPageComponents => newPageComponents;


    // Deprecated function, to be removed
    //public void AddComponentToPage(MoveablePageComponent addedComponent)
    //{
    //    if (pageComponents.Contains(addedComponent)) return;

    //    pageComponents.Add(addedComponent);
    //    if(addedComponent.GetType() == typeof(ScrapbookSticker))
    //    {
    //        addedComponent.transform.SetParent(overlayLayer, true);
    //        return;
    //    }
    //    addedComponent.transform.SetParent(defaultLayer, true);
    //}
    public bool AddComponentToPage(PageComponent addedComponent)
    {
        if (newPageComponents.Contains(addedComponent)) return false;

        newPageComponents.Add(addedComponent);
        if (addedComponent.GetType() == typeof(ScrapbookSticker))
        {
            addedComponent.transform.SetParent(overlayLayer, true);
            return true;
        }
        addedComponent.transform.SetParent(defaultLayer, true);
        return true;
    }

    // Deprecated function, to be removed
    //public void RemoveComponentFromPage(MoveablePageComponent removedComponent)
    //{
    //    if (pageComponents.Contains(removedComponent))
    //    {
    //        pageComponents.Remove(removedComponent);
    //    }
    //}

    public void RemoveComponentFromPage(PageComponent removedComponent)
    {
        if (newPageComponents.Contains(removedComponent))
        {
            newPageComponents.Remove(removedComponent);
        }
    }

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (AddComponentToPage(component))
        {
            component.SetInteractor(this);
        }
        return true;
    }
    public override void RemoveFromInteractor(PageComponent component)
    {
        RemoveComponentFromPage(component);
    }
}
