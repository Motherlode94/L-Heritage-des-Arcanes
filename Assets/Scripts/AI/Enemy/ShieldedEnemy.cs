using UnityEngine;

public class ShieldedEnemy : MonoBehaviour
{
    public int health = 100;
    public bool hasShield = true;
    public int shieldStrength = 50;
    public float shieldRegenRate = 10f;  // Points de bouclier régénérés par seconde
    public float shieldRegenDelay = 5f;  // Temps avant de commencer la régénération

    private float shieldRegenTimer;

    void Update()
    {
        // Régénérer le bouclier si l'ennemi a encore un bouclier et que celui-ci n'est pas plein
        if (hasShield && shieldStrength < 50)
        {
            shieldRegenTimer += Time.deltaTime;

            if (shieldRegenTimer >= shieldRegenDelay)
            {
                RegenerateShield();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            shieldStrength -= damage;
            shieldRegenTimer = 0f;  // Réinitialiser le timer de régénération

            if (shieldStrength <= 0)
            {
                hasShield = false;
                Debug.Log("Shield broken!");
                TriggerShieldBreakEffect();
            }
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

    // Régénération du bouclier
    private void RegenerateShield()
    {
        shieldStrength += Mathf.FloorToInt(shieldRegenRate * Time.deltaTime);
        if (shieldStrength > 50)
        {
            shieldStrength = 50;
        }
        Debug.Log("Shield regenerating: " + shieldStrength);
    }

    // Effet visuel/audio de la destruction du bouclier
    private void TriggerShieldBreakEffect()
    {
        // Ici, vous pouvez ajouter un effet visuel ou un son
        Debug.Log("Shield break effect triggered!");
    }

    // Quand l'ennemi meurt
    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Enemy defeated!");
    }
}
