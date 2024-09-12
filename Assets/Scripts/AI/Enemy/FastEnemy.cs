using UnityEngine;

public class FastEnemy : MonoBehaviour
{
    public float speed = 10f;
    public int health = 30;
    public float chargeDistance = 5f; // Distance à partir de laquelle il charge le joueur
    public float evadeDistance = 3f; // Distance pour esquiver les attaques

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < chargeDistance)
        {
            ChargePlayer();
        }
        else if (distanceToPlayer < evadeDistance)
        {
            Evade();
        }
        else
        {
            FollowPlayer();
        }
    }

    void ChargePlayer()
    {
        Vector3 chargeDirection = (player.position - transform.position).normalized;
        transform.position += chargeDirection * speed * Time.deltaTime;
        Debug.Log("Fast enemy charging at player!");
    }

    void Evade()
    {
        Vector3 evadeDirection = (transform.position - player.position).normalized;
        transform.position += evadeDirection * speed * Time.deltaTime;
        Debug.Log("Fast enemy evading!");
    }

    void FollowPlayer()
    {
        Vector3 followDirection = (player.position - transform.position).normalized;
        transform.position += followDirection * (speed / 2) * Time.deltaTime; // Se déplacer moins vite en mode normal
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
        Debug.Log("Fast enemy defeated!");
    }
}
