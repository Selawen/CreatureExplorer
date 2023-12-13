using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BerryPouch : MonoBehaviour
{
    public bool HoldingBerry { get; set; }

    [SerializeField] private Transform[] berrySpots;
    [SerializeField] private Button berryButtonPrefab;

    private Inventory<Throwable> berryPouch;

    public void Unlock()
    {
        berryPouch = new Inventory<Throwable>((ushort)berrySpots.Length);
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
                    berryButton.onClick.AddListener(()=>
                    {
                        if (!HoldingBerry)
                        {
                            Debug.Log("Should hold the berry at this point");
                            berryPouch.RemoveItemFromInventory(berry);
                        }
                    });
                    return true;
                }
            }
        }
        return false;
    }

}
