using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagePicture : MoveablePageComponent, IPointerEnterHandler, IPointerExitHandler
{
    public PictureInfo PictureInfo { get; private set; }
    public System.Action OnPictureClicked;

    [SerializeField] private Image pictureGraphic;
    [SerializeField] private float pageScaleFactor = 2.5f;

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

    public override void OnDrag(PointerEventData eventData)
    {
        if (!PlacedOnPage)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float componentX = Mathf.Clamp(_componentTransform.anchoredPosition.x + eventData.delta.x, halfWidth * _componentTransform.localScale.x, _parentTransform.rect.xMax * 2 - halfWidth * _componentTransform.localScale.x);
            float componentY = Mathf.Clamp(_componentTransform.anchoredPosition.y + eventData.delta.y, _parentTransform.rect.yMax * -2 + halfHeight * _componentTransform.localScale.y, -halfHeight * _componentTransform.localScale.y);

            _componentTransform.anchoredPosition = new Vector2(componentX, componentY);
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
