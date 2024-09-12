using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    private InventoryUI inventoryUI;

    void Start()
    {
        inventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Loot"))
        {
            PickUpItem(other.gameObject);
        }
    }

    void PickUpItem(GameObject item)
    {
        inventory.Add(item);
        Destroy(item);
        Debug.Log("Item added to inventory!");
        inventoryUI.UpdateInventoryUI(); // Mettre à jour l'UI à chaque ramassage d'objet
    }
}
