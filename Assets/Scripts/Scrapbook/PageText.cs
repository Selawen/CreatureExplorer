using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageText : MoveablePageComponent
{
    private TMP_InputField textField;

    private void Awake()
    {
        textField = GetComponent<TMP_InputField>();
        if (!textField)
        {
            throw new System.NullReferenceException("No TextMeshPro InputField found on object named: " + name);
        }
    }
    private void Start()
    {
        if (!_componentTransform)
        {
            _componentTransform = textField.GetComponent<RectTransform>();
            _parentTransform = _componentTransform.parent.GetComponent<RectTransform>();
            _componentGraphic = _componentTransform.GetComponent<Image>();
        }
        SetHalfSizes();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        textField.interactable = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        textField.interactable = true;
    }

}
