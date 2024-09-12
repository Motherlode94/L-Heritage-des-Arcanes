using UnityEngine;

public class ExplosiveEnemy : MonoBehaviour
{
    public float explosionRadius = 5f; // Rayon de l'explosion
    public int explosionDamage = 50;
    public int health = 40;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < explosionRadius)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Infliger des dégâts au joueur et à d'autres objets proches
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Player"))
            {
                // Infliger des dégâts au joueur
                Debug.Log("Player takes explosion damage!");
            }
        }

        Destroy(gameObject);
        Debug.Log("Explosive enemy exploded!");
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Explode();
        }
    }
}
