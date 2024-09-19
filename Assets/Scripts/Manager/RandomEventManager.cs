using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public GameObject bossEnemy;
    public float eventInterval = 30f; // Temps entre chaque événement aléatoire
    private float eventTimer;

    void Update()
    {
        eventTimer += Time.deltaTime;

        if (eventTimer >= eventInterval)
        {
            TriggerRandomEvent();
            eventTimer = 0f;
        }
    }

    void TriggerRandomEvent()
    {
        int eventType = Random.Range(0, 3); // Augmentation du nombre d'événements potentiels

        switch (eventType)
        {
            case 0:
                TriggerStormEvent();
                break;
            case 1:
                TriggerBossEvent();
                break;
            default:
                Debug.Log("No event triggered this time.");
                break;
        }
    }

    void TriggerStormEvent()
    {
        Debug.Log("Storm event triggered!");
        // Code pour activer une tempête, par exemple : changement de météo, dégâts aléatoires
    }

    void TriggerBossEvent()
    {
        Debug.Log("Boss event triggered!");
        Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        Instantiate(bossEnemy, randomPosition, Quaternion.identity); // Spawn à une position aléatoire
    }
}
