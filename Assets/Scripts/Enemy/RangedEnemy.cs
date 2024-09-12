using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public GameObject projectilePrefab; // Le projectile à tirer
    public float attackRange = 10f; // Portée d'attaque
    public float fireRate = 1f; // Temps entre les tirs
    public int health = 50;

    private Transform player;
    private float nextFireTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FireProjectile()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(direction));
        Debug.Log("Projectile fired at player!");
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
        Debug.Log("Ranged enemy defeated!");
    }
}
