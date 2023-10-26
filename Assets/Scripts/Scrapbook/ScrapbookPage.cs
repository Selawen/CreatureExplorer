using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapbookPage : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text pageNumberText;

    private List<MoveablePageComponent> pageComponents = new();

    public void SetPageNumber(int pageNumber)
    {
        pageNumberText.text = pageNumber.ToString();
    }

    public void AddComponentToPage(MoveablePageComponent addedComponent)
    {
        pageComponents.Add(addedComponent);
    }

    public void RemoveComponentFromPage(MoveablePageComponent removedComponent)
    {
        if (pageComponents.Contains(removedComponent))
        {
            pageComponents.Remove(removedComponent);
        }
    }

}
