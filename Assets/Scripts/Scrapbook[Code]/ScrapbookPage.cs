using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapbookPage : PageComponentInteractor
{
    [SerializeField] private TMPro.TMP_Text pageNumberText;
    [SerializeField] private RectTransform textLayer;
    [SerializeField] private RectTransform photoLayer;
    [SerializeField] private RectTransform overlayLayer;

    private List<PageComponent> pageComponents = new();

    public void SetPageNumber(int pageNumber) => pageNumberText.text = pageNumber.ToString();

    public void AddComponentToPage(PageComponent addedComponent)
    {
        if (!pageComponents.Contains(addedComponent))
        {
            pageComponents.Add(addedComponent);
        }

        if (addedComponent.GetType() == typeof(ScrapbookSticker))
        {
            addedComponent.transform.SetParent(overlayLayer, true);
        }
        else if(addedComponent.GetType() == typeof(PageText))
        {
            addedComponent.transform.SetParent(textLayer, true);
        }
        else
        {
            addedComponent.transform.SetParent(photoLayer, true);
        }
    }

    public void RemoveComponentFromPage(PageComponent removedComponent)
    {
        if (pageComponents.Contains(removedComponent))
        {
            pageComponents.Remove(removedComponent);
        }
    }

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        AddComponentToPage(component);

        return true;
    }
    public override void RemoveFromInteractor(PageComponent component)
    {
        RemoveComponentFromPage(component);
    }
}
