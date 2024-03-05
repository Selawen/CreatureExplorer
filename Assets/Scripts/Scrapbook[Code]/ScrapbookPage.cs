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
        else if (addedComponent.GetType() == typeof(PagePicture))
        {
            PagePicture picture = addedComponent as PagePicture;
            if (!picture.evaluated)
            {
                picture.evaluated = true;

                if(StaticQuestHandler.OnPictureInScrapbook != null)
                    StaticQuestHandler.OnPictureInScrapbook.Invoke(picture);
            }

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

    public void CheckPicsForQuest()
    {
        foreach (PagePicture picture in photoLayer.GetComponentsInChildren<PagePicture>())
        {
            StaticQuestHandler.OnPictureInScrapbook.Invoke(picture);
        }
    }
}
