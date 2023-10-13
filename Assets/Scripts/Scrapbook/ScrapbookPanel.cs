using UnityEngine;
using UnityEngine.EventSystems;

public class ScrapbookPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float dockedXValue;
    [SerializeField] private float hoverXValue;
    [SerializeField] private float extendedXValue;

    //[SerializeField] private Scrapbook scrapbook;

    private RectTransform rect;
    private bool extended;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (!rect)
        {
            throw new System.NullReferenceException("There's no Rect Transform on the scrapbook panel! This is not allowed!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rect.anchoredPosition = new Vector2(extended ? dockedXValue : extendedXValue, rect.anchoredPosition.y);
        extended = !extended;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!extended)
        {
            rect.anchoredPosition = new Vector2(hoverXValue, rect.anchoredPosition.y);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!extended)
        {
            rect.anchoredPosition = new Vector2(dockedXValue, rect.anchoredPosition.y);
        }
    }

    public void DockPanel()
    {
        rect.anchoredPosition = new Vector2(dockedXValue, rect.anchoredPosition.y);
    }

}
