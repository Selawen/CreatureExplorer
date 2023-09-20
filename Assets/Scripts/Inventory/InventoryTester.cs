using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    private Inventory<GameObject> inventory;

    private void Awake()
    {
        inventory = new Inventory<GameObject>(4);
    }
    public void GetLeftClickInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                if (inventory.AddItemToInventory(hit.transform.gameObject))
                {
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
    }
    public void GetRightClickInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started && inventory.GetItemCount() > 0)
        {
            GameObject g = inventory.GetContents()[0];

            if (inventory.RemoveItemFromInventory(0))
            {
                g.SetActive(true);    
            }
        }
    }


}
