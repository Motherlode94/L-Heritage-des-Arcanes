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
        int eventType = Random.Range(0, 2);

        if (eventType == 0)
        {
            Debug.Log("Storm event triggered!");
            // Code pour activer une tempête
        }
        else if (eventType == 1)
        {
            Debug.Log("Boss event triggered!");
            Instantiate(bossEnemy, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
