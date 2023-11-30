using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagePicture : PageComponent, IPointerClickHandler
{
    public PictureInfo PictureInfo { get; private set; }

    public static System.Action<PagePicture> OnPictureClicked;
    public static System.Action OnBeginPictureDrag;
    public static System.Action OnEndPictureDrag;

    [SerializeField] private Image pictureGraphic;
    [SerializeField] private float pageScaleFactor = 2.5f;

    private bool dragging;
    private Transform stepBackParent;
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

        SetStepBackParent();

        if(parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }
        transform.SetParent(parentCanvas.transform);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        OnEndPictureDrag?.Invoke();
        dragging = false;
    }

    public void SetStepBackParent()
    {
        stepBackParent = previousParent;
    }
    public void OnRevert()
    {
        PageComponentInteractor interactor = stepBackParent.GetComponentInParent(typeof(PageComponentInteractor), true) as PageComponentInteractor;

        if (interactor == null)
        {
            Debug.Log(stepBackParent.name);
            throw new System.NullReferenceException("Something went wrong, in the parent transform or one of their parent's transforms, there should be a Page Component Interactor");
        }
        if (interactor.OnComponentDroppedOn(this))
        {
            SetInteractor(interactor);
        }
        else
        {
            Debug.Log(interactor.name);
            Debug.LogError("For some reason, the picture cannot be return to it's parent, this shouldn't happen");
        }
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
