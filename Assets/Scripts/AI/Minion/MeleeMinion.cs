using UnityEngine;

public class MeleeMinion : MonoBehaviour
{
    public float speed = 3f;
    public int health = 20;
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        FollowPlayer();

        // Vérifier si le minion est à portée d'attaque
        if (Vector3.Distance(transform.position, player.position) <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void AttackPlayer()
    {
        Debug.Log("Melee Minion attacks the player, dealing " + damage + " damage!");
        // Ajoutez ici la logique pour infliger des dégâts au joueur
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
        Debug.Log("Melee Minion defeated!");
    }
}
