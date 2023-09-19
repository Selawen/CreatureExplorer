using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapbookPage : MonoBehaviour
{
    private List<MoveablePageComponent> pageComponents = new();

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
