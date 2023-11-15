using UnityEngine;
using UnityEngine.EventSystems;

// A sticker class for the scrapbook. May have added functionality later.
public class ScrapbookSticker : PageComponent, IPointerUpHandler, IDragHandler
{
    public bool IsTemplate = true;

    //private RectTransform rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    rectTransform.anchoredPosition = eventData.position;
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsTemplate)
        {
            // Instantiate a new sticker on this spot, without additional functions.
            Vector3 dropLocation = _rectTransform.anchoredPosition;
            ScrapbookSticker newSticker = Instantiate(this, dropLocation, Quaternion.identity);
            newSticker.IsTemplate = false;
        } 
    }
}
