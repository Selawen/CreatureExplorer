using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BerryPouch : MonoBehaviour
{
    public bool HoldingBerry { get; set; }

    [SerializeField] private GameObject pouchGraphic;
    [SerializeField] private Transform[] berrySpots;
    [SerializeField] private Button berryButtonPrefab;

    private CC_PlayerController controller;
    private Inventory<Throwable> berryPouch;
    private PlayerInput input;

    private void Awake()
    {
        pouchGraphic.SetActive(false);
        controller = GetComponentInParent<CC_PlayerController>();
        input = GetComponentInParent<PlayerInput>();
    }
    public void Unlock()
    {
        berryPouch = new Inventory<Throwable>((ushort)berrySpots.Length);
    }

    public void OpenPouch()
    {
        if(berryPouch != null)
        {
            pouchGraphic.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            input.SwitchCurrentActionMap("Pouch");
        }
    }

    public void ClosePouch()
    {
        pouchGraphic.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        input.SwitchCurrentActionMap("Overworld");
    }

    public bool AddBerry(Throwable berry)
    {
        if(berryPouch != null && berryPouch.AddItemToInventory(berry))
        {
            foreach(Transform t in berrySpots)
            {
                if (t.childCount == 0)
                {
                    // Add a berry graphic button here
                    Button berryButton = Instantiate(berryButtonPrefab, t);
                    berryButton.GetComponentInChildren<Image>().sprite = berry.InventoryGraphic;
                    SpriteState buttonState = berryButton.spriteState;
                    buttonState.highlightedSprite = berry.HoverGraphic;
                    buttonState.selectedSprite = berry.HoverGraphic;
                    berryButton.spriteState = buttonState;
                    berryButton.onClick.AddListener(()=>
                    {
                        if (!HoldingBerry)
                        {
                            HoldingBerry = true;
                            berryPouch.RemoveItemFromInventory(berry);
                            controller.ReceiveRetrievedBerry(berry);
                            ClosePouch();
                            Destroy(berryButton.gameObject);
                        }
                    });
                    return true;
                }
            }
        }
        return false;
    }

}
