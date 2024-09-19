using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public InventoryItem item;  // L'item à ramasser

    public void Interact()
    {
        Debug.Log("Vous avez ramassé " + item.quantity + " " + item.itemName + "(s)");
        // Logique pour ajouter l'objet à l'inventaire
        FindObjectOfType<Inventory>().PickUpItem(item);
        Destroy(gameObject);  // Détruire l'objet après ramassage
    }

    // Méthode pour obtenir l'item associé à ce pickup
    public InventoryItem GetInventoryItem()
    {
        return item;
    }
}
