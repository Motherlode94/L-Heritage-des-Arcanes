using UnityEngine;

public class BossAI : MonoBehaviour
{
    public int health = 500;
    public GameObject specialAttackPrefab;
    public GameObject enragedAttackPrefab;
    public GameObject minionPrefab; // Minion à faire apparaître
    public Transform[] spawnPoints; // Points de spawn pour les minions

    private int currentPhase = 1;

    void Update()
    {
        if (health <= 500 && currentPhase == 1)
        {
            EnterPhase2();
        }
        if (health <= 250 && currentPhase == 2)
        {
            EnterPhase3();
        }
    }

    // Phase 2 : Apparition de minions et attaque spéciale
    void EnterPhase2()
    {
        currentPhase = 2;
        Debug.Log("Boss enters Phase 2!");

        // Activer une attaque spéciale
        InvokeRepeating("SpecialAttack", 0f, 5f);

        // Faire apparaître des minions pour assister le boss
        SpawnMinions(3); // Faire apparaître 3 minions
    }

    // Phase 3 : Minions supplémentaires et attaque enragée
    void EnterPhase3()
    {
        currentPhase = 3;
        Debug.Log("Boss enters Phase 3!");

        // Activer une attaque enragée
        InvokeRepeating("EnragedAttack", 0f, 3f);

        // Faire apparaître des minions guérisseurs
        SpawnMinions(2, healerMinionPrefab); // Appelons des minions guérisseurs


        // Faire apparaître plus de minions pour assister le boss
        SpawnMinions(5); // Faire apparaître 5 minions

        // Déclencher un événement de mur de feu
        Invoke("SpawnFireWalls", 2f);
    }

    void SpawnFireWalls()
    {
        // Faire apparaître des murs de feu à des positions fixes ou aléatoires
        foreach (Transform spawnPoint in spawnPoints)
       {
        Instantiate(fireWallPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Fire Wall spawned!");
       }
    }

    void SpecialAttack()
    {
        Vector3 attackPosition = transform.position + transform.forward * 5f;
        Instantiate(specialAttackPrefab, attackPosition, Quaternion.identity);
        Debug.Log("Boss uses Special Attack!");
    }

    void EnragedAttack()
    {
        Vector3 attackPosition = transform.position + transform.forward * 5f;
        Instantiate(enragedAttackPrefab, attackPosition, Quaternion.identity);
        Debug.Log("Boss uses Enraged Attack!");
    }

    // Faire apparaître des minions pour aider le boss
    void SpawnMinions(int numberOfMinions)
    {
        for (int i = 0; i < numberOfMinions; i++)
        {
            // Choisir un point de spawn aléatoire
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(minionPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Minion spawned to assist the boss!");
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
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }
}
