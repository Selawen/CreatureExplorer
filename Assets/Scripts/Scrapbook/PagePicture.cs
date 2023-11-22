using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagePicture : PageComponent, IPointerClickHandler
{
    public PictureInfo PictureInfo { get; private set; }

    public static System.Action<PagePicture> OnPictureClicked;
    public static System.Action OnBeginPictureDrag;

    [SerializeField] private Image pictureGraphic;
    [SerializeField] private float pageScaleFactor = 2.5f;

    private bool dragging;
    private Canvas parentCanvas;


    private void Awake()
    {
        if(_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void SetPicture(Sprite pictureSprite)
    {
        pictureGraphic.sprite = pictureSprite;
    }

    public void LinkPictureInformation(PictureInfo information)
    {
        PictureInfo = information;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        base.OnBeginDrag(eventData);
        OnBeginPictureDrag?.Invoke();
        if(parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }
        transform.SetParent(parentCanvas.transform);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        dragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //return;

        if (eventData.button == PointerEventData.InputButton.Left && !dragging)
        {
            OnPictureClicked?.Invoke(this);
            return;
        }
    }
}
