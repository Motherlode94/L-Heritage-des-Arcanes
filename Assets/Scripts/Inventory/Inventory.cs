using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite itemIcon;
    public int quantity;
    public string description;
}

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();
    private InventoryUI inventoryUI;

    void Start()
    {
        inventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Loot"))
        {
            PickUpItem(other.GetComponent<ItemPickup>().GetInventoryItem());
        }
    }

    // Modifier la méthode PickUpItem pour la rendre publique
    public void PickUpItem(InventoryItem item)
    {
        InventoryItem existingItem = inventory.Find(i => i.itemName == item.itemName);
        if (existingItem != null)
        {
            existingItem.quantity += item.quantity; // Ajouter la quantité si l'objet est déjà présent
        }
        else
        {
            inventory.Add(item); // Ajouter un nouvel objet
        }
        Debug.Log(item.itemName + " ajouté à l'inventaire !");
        inventoryUI.UpdateInventoryUI();
    }
}
