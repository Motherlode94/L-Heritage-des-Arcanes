using UnityEngine;

[System.Serializable]
public class LootItem
{
    public GameObject item;
    public float dropChance;  // Chance individuelle de cet objet
}

public class LootSystem : MonoBehaviour
{
    public LootItem[] lootItems;

    public void DropLoot()
    {
        float randomValue = Random.value;
        float cumulativeChance = 0f;

        foreach (LootItem lootItem in lootItems)
        {
            cumulativeChance += lootItem.dropChance;
            if (randomValue <= cumulativeChance)
            {
                Instantiate(lootItem.item, transform.position, Quaternion.identity);
                break;
            }
        }
    }
}
