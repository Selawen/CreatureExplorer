using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageText : MoveablePageComponent
{
    public TMP_InputField TextField { get; private set; }

    private Vector3 startPosition;
    private ScrapbookPage linkedPage;


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
        startPosition = _componentTransform.position;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        TextField.interactable = true;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            List<RaycastResult> results = new();
            Scrapbook.Instance.Raycaster.Raycast(eventData, results);

            if (results.Count == 0 || !results[0].gameObject.CompareTag("PictureKeeper"))
            {
                if(linkedPage != null)
                {
                    linkedPage.RemoveComponentFromPage(this);
                }
                Destroy(gameObject);
                return;
            }
            RaycastResult firstResult = results[0];
            if (firstResult.gameObject.TryGetComponent(out ScrapbookPage page))
            {
                page.AddComponentToPage(this);
                linkedPage = page;
                PlacedOnPage = true;
                return;
            }
            _componentTransform.SetParent(firstResult.gameObject.transform, true);
        }
    }

}
