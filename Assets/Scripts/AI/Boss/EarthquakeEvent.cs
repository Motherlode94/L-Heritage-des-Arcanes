using UnityEngine;

public class EarthquakeEvent : MonoBehaviour
{
    public GameObject fallingRockPrefab; // Préfab du rocher qui tombe
    public float rockSpawnRate = 2f; // Intervalle entre chaque rocher
    public float eventDuration = 10f; // Durée de l'événement

    private float nextRockTime;
    private float eventEndTime;

    void Start()
    {
        eventEndTime = Time.time + eventDuration;
    }

    void Update()
    {
        if (Time.time <= eventEndTime && Time.time >= nextRockTime)
        {
            SpawnFallingRock();
            nextRockTime = Time.time + rockSpawnRate;
        }
    }

    void SpawnFallingRock()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10));
        Instantiate(fallingRockPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("A falling rock appears!");
    }
}
