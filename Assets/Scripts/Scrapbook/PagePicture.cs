using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagePicture : MoveablePageComponent, IPointerEnterHandler, IPointerExitHandler
{
    public PictureInfo PictureInfo { get; private set; }
    public System.Action OnPictureClicked;

    [SerializeField] private Image pictureGraphic;

    private bool placedOnPage;

    private void Start()
    {
        SetHalfSizes();
        OnPictureClicked = SelectForPlacement;
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
        if (eventData.button == PointerEventData.InputButton.Left && !placedOnPage)
        {
            OnPictureClicked?.Invoke();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right && placedOnPage)
        {
            RemovePicture();
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!placedOnPage)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float componentX = Mathf.Clamp(_componentTransform.anchoredPosition.x + eventData.delta.x, halfWidth * _componentTransform.localScale.x, _parentTransform.rect.xMax * 2 - halfWidth * _componentTransform.localScale.x);
            float componentY = Mathf.Clamp(_componentTransform.anchoredPosition.y + eventData.delta.y, _parentTransform.rect.yMax * -2 + halfHeight * _componentTransform.localScale.y, -halfHeight * _componentTransform.localScale.y);

            _componentTransform.anchoredPosition = new Vector2(componentX, componentY);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            _componentTransform.Rotate(new Vector3(0, 0, eventData.delta.y));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Hovering over the picture");
        //foreach(IIdentifiable i in PictureInfo.PictureObjects)
        //{
        //    Debug.Log($"This picture contains a {i.GivenName}");
        //    Debug.Log($"It is described as {i.GivenDescription}");
        //}
    }

    public void SelectForPlacement()
    {
        ScrapbookPage page = Scrapbook.Instance.CurrentPage;
        page.AddComponentToPage(this);
        transform.SetParent(page.transform, false);
        _parentTransform = page.GetComponent<RectTransform>();
        _componentTransform.anchoredPosition = new Vector3(_parentTransform.rect.width * 0.5f, -_parentTransform.rect.height * 0.5f, 0);
        placedOnPage = true;
        // To do: close the placement panel (event?)
    }


    private void RemovePicture()
    {
        if (!Scrapbook.Instance.AddPictureToCollection(this))
        {
            return;
        }
        _componentTransform.rotation = Quaternion.identity;
        Scrapbook.Instance.CurrentPage.RemoveComponentFromPage(this);
        placedOnPage = false;
    }
}
