using UnityEngine;

public class HealerMinion : MonoBehaviour
{
    public float healRange = 10f;
    public int healAmount = 10;
    public float healCooldown = 5f;
    private float nextHealTime;
    private GameObject boss;

    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss"); // Le boss est identifié avec le tag "Boss"
    }

    void Update()
    {
        // Si le boss est dans la portée, soigner après un certain temps
        if (Time.time >= nextHealTime && Vector3.Distance(transform.position, boss.transform.position) <= healRange)
        {
            HealBoss();
            nextHealTime = Time.time + healCooldown;
        }
    }

    void HealBoss()
    {
        BossAI bossAI = boss.GetComponent<BossAI>();
        if (bossAI != null)
        {
            bossAI.TakeDamage(-healAmount); // Soin = annulation de dégâts
            Debug.Log("Boss healed by minion!");
        }
    }

    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
        Debug.Log("Healer minion defeated!");
    }
}
