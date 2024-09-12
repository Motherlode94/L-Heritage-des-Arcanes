using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform player;
    public float followDistance = 3f;
    public int healAmount = 10;
    private float healCooldown = 5f;
    private float healTimer;

    void Update()
    {
        FollowPlayer();

        healTimer += Time.deltaTime;
        if (healTimer >= healCooldown)
        {
            HealPlayer();
            healTimer = 0f;
        }
    }

    void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > followDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, 5f * Time.deltaTime);
        }
    }

    void HealPlayer()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        playerStats.UpgradeHealth(healAmount);
        Debug.Log("Player healed by companion!");
    }
}
