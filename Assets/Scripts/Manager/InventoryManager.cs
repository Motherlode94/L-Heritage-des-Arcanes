using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    public GameObject inventoryPanel; // Le panneau d'inventaire assigné via l'Inspector
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    private bool isInventoryOpen = false;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.OpenInventory.performed += ToggleInventory;
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    // Méthode pour ouvrir/fermer l'inventaire
    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryPanel != null)
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);

            if (isInventoryOpen)
            {
                // Pause le jeu en mettant le temps à 0
                Time.timeScale = 0f;
                // Optionnel : désactiver le contrôle du joueur
                // playerControls.Disable(); // Si vous voulez aussi désactiver les mouvements du joueur
            }
            else
            {
                // Reprendre le jeu
                Time.timeScale = 1f;
                // Optionnel : réactiver le contrôle du joueur
                // playerControls.Enable(); // Si vous aviez désactivé les mouvements du joueur
            }
        }
        else
        {
            Debug.LogError("inventoryPanel n'est pas assigné dans l'inspecteur.");
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
