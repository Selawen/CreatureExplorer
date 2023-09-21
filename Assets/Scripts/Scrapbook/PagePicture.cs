using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagePicture : MoveablePageComponent, IPointerClickHandler
{
    [SerializeField] private Image pictureGraphic;

    private bool placedOnPage;

    private void Start()
    {
        SetHalfSizes();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !placedOnPage)
        {
            SelectForPlacement();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right && placedOnPage)
        {
            RemovePicture();
        }
    }

    public void SetPicture(Sprite pictureSprite)
    {
        pictureGraphic.sprite = pictureSprite;
    }

    private void SelectForPlacement()
    {
        // To do: move the component to the current page of the scrapbook and remove it from the panel.
        // Also close the placement panel (event?)
    }

    private void RemovePicture()
    {
        // To do: bring the picture back to the collection, if it's not full yet.
    }
}
