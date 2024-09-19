using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; // Panneau UI de l'inventaire
    public Transform itemSlotContainer; // Container pour les slots d'items
    public GameObject itemSlotPrefab; // Préfab pour afficher un slot d'item

    private Inventory inventory;

    void Start()
    {
        // Obtenir le script Inventory du joueur
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // Nettoyer les anciens slots
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // Ajouter les objets de l'inventaire à l'UI
        foreach (InventoryItem item in inventory.inventory)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, itemSlotContainer);
            itemSlot.GetComponentInChildren<Text>().text = item.itemName; // Afficher le nom de l'item
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf); // Afficher ou cacher l'inventaire
    }
}
