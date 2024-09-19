using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemName, int quantity)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += quantity;
        }
        else
        {
            inventory[itemName] = quantity;
        }

        Debug.Log(itemName + " ajouté à l'inventaire. Quantité : " + inventory[itemName]);
    }

    public bool HasItem(string itemName, int quantity)
    {
        return inventory.ContainsKey(itemName) && inventory[itemName] >= quantity;
    }

    public void RemoveItem(string itemName, int quantity)
    {
        if (HasItem(itemName, quantity))
        {
            inventory[itemName] -= quantity;
            if (inventory[itemName] <= 0)
            {
                inventory.Remove(itemName);
            }
        }
    }
}
