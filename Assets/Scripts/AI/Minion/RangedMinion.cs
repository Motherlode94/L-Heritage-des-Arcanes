using UnityEngine;

public class RangedMinion : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackRange = 10f;
    public float fireRate = 1f;
    private Transform player;
    private float nextFireTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Si le joueur est dans la portée et qu'il est temps de tirer
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
        Debug.Log("Ranged minion fires at player!");
    }

    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
        Debug.Log("Ranged minion defeated!");
    }
}
