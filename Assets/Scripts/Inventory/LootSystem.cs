using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public GameObject[] lootItems; // Liste des objets à looter
    public float dropChance = 0.5f; // Chance de loot (50%)

    public void DropLoot()
    {
        if (Random.value <= dropChance)
        {
            int randomIndex = Random.Range(0, lootItems.Length);
            Instantiate(lootItems[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
