using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapbookPage : PageComponentInteractor
{
    [SerializeField] private TMPro.TMP_Text pageNumberText;
    [SerializeField] private RectTransform defaultLayer;
    [SerializeField] private RectTransform overlayLayer;

    private List<PageComponent> newPageComponents = new();

    public void SetPageNumber(int pageNumber) => pageNumberText.text = pageNumber.ToString();

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

    public void RemoveComponentFromPage(PageComponent removedComponent)
    {
        if (newPageComponents.Contains(removedComponent))
        {
            newPageComponents.Remove(removedComponent);
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
