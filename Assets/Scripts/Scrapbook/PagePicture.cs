using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagePicture : MoveablePageComponent, IPointerEnterHandler, IPointerExitHandler
{
    public PictureInfo PictureInfo { get; private set; }
    public System.Action OnPictureClicked;

    [SerializeField] private Image pictureGraphic;
    [SerializeField] private float pageScaleFactor = 2.5f;

    private Image pictureBackground;
    private Vector3 startPosition;


    private void Awake()
    {
        pictureBackground = GetComponent<Image>();

        _componentTransform = GetComponent<RectTransform>();
        _componentGraphic = pictureBackground;
    }
    private void Start()
    {
        SetHalfSizes();
        OnPictureClicked += SelectForPlacement;
    }

    public void SetPicture(Sprite pictureSprite)
    {
        pictureGraphic.sprite = pictureSprite;
    }

    public void LinkPictureInformation(PictureInfo information)
    {
        PictureInfo = information;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        return;

        if (eventData.button == PointerEventData.InputButton.Left && !PlacedOnPage)
        {
            OnPictureClicked?.Invoke();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (PlacedOnPage)
            {
                RemovePicture();
                return;
            }
            Scrapbook.Instance.RemovePictureFromCollection(this);
            Destroy(gameObject);
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            pictureBackground.raycastTarget = false;
            startPosition = _componentTransform.position;
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        //if (!PlacedOnPage)
        //    return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            //float componentX = Mathf.Clamp(_componentTransform.anchoredPosition.x + eventData.delta.x, halfWidth * _componentTransform.localScale.x, _parentTransform.rect.xMax * 2 - halfWidth * _componentTransform.localScale.x);
            //float componentY = Mathf.Clamp(_componentTransform.anchoredPosition.y + eventData.delta.y, _parentTransform.rect.yMax * -2 + halfHeight * _componentTransform.localScale.y, -halfHeight * _componentTransform.localScale.y);

            _componentTransform.position = new Vector2(x, y);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            List<RaycastResult> results = new();
            Scrapbook.Instance.Raycaster.Raycast(eventData, results);

            pictureBackground.raycastTarget = true;

            if (results.Count == 0 || !results[0].gameObject.CompareTag("PictureKeeper"))
            {
                _componentTransform.position = startPosition;
                return;
            }
            RaycastResult firstResult = results[0];
            if(firstResult.gameObject.TryGetComponent(out ScrapbookPage page))
            {
                page.AddComponentToPage(this);
                return;
            }
            _componentTransform.SetParent(firstResult.gameObject.transform, true);
        }
    }
    public void SelectForPlacement()
    {
        ScrapbookPage page = Scrapbook.Instance.CurrentPage;
        page.AddComponentToPage(this);
        Scrapbook.Instance.RemovePictureFromCollection(this);
        transform.SetParent(page.transform, false);
        _parentTransform = page.GetComponent<RectTransform>();
        _componentTransform.anchoredPosition = new Vector3(_parentTransform.rect.width * 0.5f, -_parentTransform.rect.height * 0.5f, 0);
        _componentTransform.localScale = Vector3.one * pageScaleFactor;
        PlacedOnPage = true;
        // To do: close the placement panel (event?)
    }


    private void RemovePicture()
    {
        if (!Scrapbook.Instance.AddPictureToCollection(this))
        {
            return;
        }
        _componentTransform.rotation = Quaternion.identity;
        _componentTransform.localScale = Vector3.one;
        Scrapbook.Instance.CurrentPage.RemoveComponentFromPage(this);
        PlacedOnPage = false;
    }
}
