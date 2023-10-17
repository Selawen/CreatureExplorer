using UnityEngine;
using UnityEngine.EventSystems;

public class ScrapbookPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static ScrapbookPanel Instance { get; private set; }

    [SerializeField] private RectTransform panel;

    [SerializeField] private float dockedXValue;
    [SerializeField] private float hoverXValue;
    [SerializeField] private float extendedXValue;

    private bool extended;

    private void Awake()
    {
        Instance = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (extended)
        {
            DockPanel();
            return;
        }
        OpenPanel();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!extended)
        {
            panel.anchoredPosition = new Vector2(hoverXValue, panel.anchoredPosition.y);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!extended)
        {
            panel.anchoredPosition = new Vector2(dockedXValue, panel.anchoredPosition.y);
        }
    }

    public void OpenPanel()
    {
        panel.anchoredPosition = new Vector2(extendedXValue, panel.anchoredPosition.y);
        extended = true;
    }

    public void DockPanel()
    {
        panel.anchoredPosition = new Vector2(dockedXValue, panel.anchoredPosition.y);
        extended = false;
    }

}
