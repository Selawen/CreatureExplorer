using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapbookPage : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text pageNumberText;
    [SerializeField] private RectTransform defaultLayer;
    [SerializeField] private RectTransform overlayLayer;

    private List<MoveablePageComponent> pageComponents = new();

    public void SetPageNumber(int pageNumber)
    {
        pageNumberText.text = pageNumber.ToString();
    }

    public void AddComponentToPage(MoveablePageComponent addedComponent)
    {
        if (pageComponents.Contains(addedComponent)) return;

        pageComponents.Add(addedComponent);
        if(addedComponent.GetType() == typeof(ScrapbookSticker))
        {
            addedComponent.transform.SetParent(overlayLayer, true);
            return;
        }
        addedComponent.transform.SetParent(defaultLayer, true);
    }

    public void RemoveComponentFromPage(MoveablePageComponent removedComponent)
    {
        if (pageComponents.Contains(removedComponent))
        {
            pageComponents.Remove(removedComponent);
        }
    }

}
