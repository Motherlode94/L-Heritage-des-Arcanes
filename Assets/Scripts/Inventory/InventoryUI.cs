using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; // Panneau UI de l'inventaire
    public Transform itemSlotContainer; // Container pour les slots d'items
    public GameObject itemSlotPrefab; // Préfab pour afficher un slot d'item

    private Inventory inventory;
    private List<GameObject> activeSlots = new List<GameObject>(); // To reuse slots instead of destroying

    void Start()
    {
        // Obtenir le script Inventory du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                UpdateInventoryUI();
            }
            else
            {
                Debug.LogError("Inventory script not found on Player!");
            }
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    public void UpdateInventoryUI()
    {
        // Désactiver les anciens slots plutôt que les détruire
        foreach (GameObject slot in activeSlots)
        {
            slot.SetActive(false);
        }

        // Créer ou réactiver des slots pour chaque objet dans l'inventaire
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            GameObject itemSlot;
            if (i < activeSlots.Count)
            {
                itemSlot = activeSlots[i];
                itemSlot.SetActive(true); // Réactiver un ancien slot
            }
            else
            {
                itemSlot = Instantiate(itemSlotPrefab, itemSlotContainer); // Créer un nouveau slot
                activeSlots.Add(itemSlot);
            }

            // Mettre à jour les infos de l'item dans le slot
            InventoryItem item = inventory.inventory[i];
            itemSlot.GetComponentInChildren<Text>().text = item.itemName; // Afficher le nom de l'item
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf); // Afficher ou cacher l'inventaire
    }
}
