using UnityEngine;

public class InvulnerableEnemy : MonoBehaviour
{
    public int health = 100;
    public string invulnerableTo = "Physical"; // Invulnérable aux attaques physiques

    public void TakeDamage(int damage, string attackType)
    {
        if (attackType == invulnerableTo)
        {
            Debug.Log("Enemy is invulnerable to " + attackType + " attacks!");
        }
        else
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Invulnerable enemy defeated!");
    }
}
