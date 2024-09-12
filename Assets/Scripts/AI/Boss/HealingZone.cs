using UnityEngine;

public class HealingZone : MonoBehaviour
{
    public int healAmount = 50;
    public float duration = 15f;

    void Start()
    {
        // La zone disparaît après une certaine durée
        Destroy(gameObject, duration);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            BossAI bossAI = other.GetComponent<BossAI>();
            if (bossAI != null)
            {
                bossAI.TakeDamage(-healAmount); // Le boss se soigne
                Debug.Log("Boss healed in healing zone!");
            }
        }
    }
}
