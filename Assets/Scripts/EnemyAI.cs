using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3f;
    public int health = 100;
    private Transform player;
    public bool followPlayer = true; // True pour suivre le joueur, false pour mouvement aléatoire

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (followPlayer)
        {
            FollowPlayer();
        }
        else
        {
            RandomMovement();
        }
    }

    void FollowPlayer()
    {
        // Mouvement pour suivre le joueur
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void RandomMovement()
    {
        // Mouvement aléatoire basique (peut être amélioré)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
        Destroy(gameObject); // L'ennemi est détruit lorsqu'il n'a plus de points de vie
        gameManager.EnemyKilled();
    }
}
