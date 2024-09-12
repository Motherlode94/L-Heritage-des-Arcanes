using UnityEngine;

public class BossSpecialAttack : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Infliger des dégâts au joueur
            Debug.Log("Player hit by special attack!");
            Destroy(gameObject);
        }
    }
}
