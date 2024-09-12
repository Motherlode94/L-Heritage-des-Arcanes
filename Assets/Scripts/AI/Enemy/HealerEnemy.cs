using UnityEngine;

public class HealerEnemy : MonoBehaviour
{
    public float healRange = 10f;
    public int healAmount = 20;
    public float healRate = 5f; // Soigne toutes les 5 secondes
    public int health = 40;

    private float nextHealTime;
    public float speed = 2f; // Vitesse de déplacement


    void Update()
    {
        if (Time.time >= nextHealTime)
        {
            HealAllies();
            nextHealTime = Time.time + healRate;
        }
    }

    public void MoveToSafePosition()
    {
        // Exemple : Se déplacer en arrière pour éviter les attaques tout en continuant de soigner
        Vector3 safePosition = transform.position + new Vector3(0, 0, -5); // Déplacement vers une position sécurisée
        transform.position = Vector3.MoveTowards(transform.position, safePosition, speed * Time.deltaTime);
        Debug.Log("Healer moving to safe position!");
    }

    void HealAllies()
    {
        Collider[] alliesInRange = Physics.OverlapSphere(transform.position, healRange);
        foreach (Collider ally in alliesInRange)
        {
            EnemyAI allyAI = ally.GetComponent<EnemyAI>();
            if (allyAI != null)
            {
                allyAI.Heal(healAmount);
                Debug.Log("Healed an ally!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Healer enemy defeated!");
    }
}
