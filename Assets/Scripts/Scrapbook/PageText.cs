using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageText : MoveablePageComponent
{
    public TMP_InputField TextField { get; private set; }

    private void Awake()
    {
        TextField = GetComponent<TMP_InputField>();
        if (!TextField)
        {
            throw new System.NullReferenceException("No TextMeshPro InputField found on object named: " + name);
        }
    }

    private void Start()
    {
        if (!_componentTransform)
        {
            _componentTransform = TextField.GetComponent<RectTransform>();
            _parentTransform = _componentTransform.parent.GetComponent<RectTransform>();
            _componentGraphic = _componentTransform.GetComponent<Image>();
        }
        SetHalfSizes();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        TextField.interactable = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        TextField.interactable = true;
    }

}
