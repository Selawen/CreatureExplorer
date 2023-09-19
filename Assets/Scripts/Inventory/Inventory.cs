using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory<T>
{
    private List<T> inventoryItems;

    private ushort capacity;

    public Inventory(ushort capacity)
    {
        inventoryItems = new();
        this.capacity = capacity;
    }

    public bool AddItemToInventory(T item)
    {
        if(inventoryItems.Count < capacity)
        {
            inventoryItems.Add(item);
            return true;
        }
        return false;
    }

    public bool RemoveItemFromInventory(T item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            return true;
        }
        return false;
    }

    public bool RemoveItemFromInventory(ushort index)
    {
        if(inventoryItems[index] != null)
        {
            inventoryItems.Remove(inventoryItems[index]);
            return true;
        }
        return false;
    }

    // Get the item at a certain index, if it exists. Otherwise, return the default.
    // Might convert this function to a bool, with an out T parameter. - Justin
    public T GetItemAtIndex(ushort index) => inventoryItems[index] != null ? inventoryItems[index] : default;

    public ushort GetItemCount() => (ushort)inventoryItems.Count;
    
    public List<T> GetContents() => inventoryItems;

}
