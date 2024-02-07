using UnityEngine;
using UnityEngine.EventSystems;

public class ScrapbookPanel : MonoBehaviour 
{ 
    //public static ScrapbookPanel Instance { get; private set; }

    [SerializeField] private RectTransform panel;

    [SerializeField] private float dockedXValue;
    [SerializeField] private float extendedXValue;

    [SerializeField] private float dockedYValue;
    [SerializeField] private float extendedYValue;

    private bool extended;

    private void Awake()
    {
        //Instance = this;
        DockPanel();
    }

    public void OpenPanel()
    {
        panel.anchoredPosition = new Vector2(extendedXValue, extendedYValue);
        extended = true;
    }

    public void DockPanel()
    {
        panel.anchoredPosition = new Vector2(dockedXValue, dockedYValue);
        extended = false;
    }

    public void TogglePanel()
    {
        if (extended)
        {
            extended = false;
            DockPanel();
            return;
        }
        extended = true;
        OpenPanel();
    }

}
