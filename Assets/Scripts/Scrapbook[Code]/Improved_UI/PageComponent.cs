using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PageComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //public System.Action OnComponentRemoved;

    protected RectTransform _rectTransform;
    protected Transform previousParent;

    private Vector3 startPosition;
    private PageComponentInteractor currentInteractor;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void SetInteractor(PageComponentInteractor interactor)
    {
        if(currentInteractor != null)
        {
            currentInteractor.RemoveFromInteractor(this);
        }
        currentInteractor = interactor;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = _rectTransform.position;
        previousParent = transform.parent;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Move(eventData.position);
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            TurnAndScale(eventData.delta);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new();
        Scrapbook.Instance.Raycaster.Raycast(eventData, results);
        foreach(RaycastResult rayResult in results)
        {
            if(rayResult.gameObject.TryGetComponent(out PageComponentInteractor pageComponentInteractor))
            {
                if (pageComponentInteractor.OnComponentDroppedOn(this))
                {
                    if(currentInteractor != pageComponentInteractor)
                    {
                        SetInteractor(pageComponentInteractor);
                    }
                    return;
                }
            }
        }
        // This should never be null if the player can drag the component
        if(previousParent != null)
        {
            transform.SetParent(previousParent);
        }
        _rectTransform.position = startPosition;
    }

    private void Move(Vector2 position)
    {
        _rectTransform.localPosition = position;
    }

    protected virtual void TurnAndScale(Vector2 delta)
    {
        _rectTransform.Rotate(new(0, 0, delta.x));
        float scale = Mathf.Clamp(_rectTransform.localScale.x + delta.y * Time.fixedDeltaTime, 0.5f, 1.5f);
        _rectTransform.localScale = Vector3.one * scale;
    }

}
