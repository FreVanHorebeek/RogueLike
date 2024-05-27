using Items;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Consumable> Items = new List<Consumable>();
    public int MaxItems;

    public bool AddItem(Consumable item)
    {
        if (Items.Count < MaxItems)
        {
            Items.Add(item);
            return true;
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add item.");
            return false;
        }
    }

    public void DropItem(Consumable item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            Destroy(item.gameObject);
        }
    }
}
