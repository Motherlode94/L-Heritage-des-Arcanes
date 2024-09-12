using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public float speed = 5f;
    public int health = 100;
    public int attackPower = 10;
    public int skillPoints = 0; // Skill points available for upgrades

    private PlayerControls playerControls;
    private InputAction upgradeAction;

    private void Awake()
    {
        playerControls = new PlayerControls();
        upgradeAction = playerControls.Player.Upgrade; // Find the "Upgrade" action in the Player input map

        if (upgradeAction != null)
        {
            upgradeAction.performed += ctx => Upgrade(); // Subscribe to the upgrade action
        }
        else
        {
            Debug.LogError("L'action 'Upgrade' n'a pas été trouvée dans PlayerControls.");
        }
    }

    private void OnEnable()
    {
        playerControls.Enable(); // Enable input actions
    }

    private void OnDisable()
    {
        playerControls.Disable(); // Disable input actions
    }

    void Upgrade()
    {
        // Check if there are enough skill points to upgrade
        if (skillPoints > 0)
        {
            UpgradeSpeed(1f); // Example: Increase speed by 1
            skillPoints--; // Reduce available skill points
            Debug.Log("Speed upgraded. Remaining skill points: " + skillPoints);
        }
        else
        {
            Debug.Log("Not enough skill points!");
        }
    }

    // Methods to upgrade different player stats
    public void UpgradeSpeed(float amount)
    {
        speed += amount;
        Debug.Log("Speed upgraded by: " + amount);
    }

    public void UpgradeHealth(int amount)
    {
        health += amount;
        Debug.Log("Health upgraded by: " + amount);
    }

    public void UpgradeAttack(int amount)
    {
        attackPower += amount;
        Debug.Log("Attack power upgraded by: " + amount);
    }

    // Method to gain skill points
    public void EarnSkillPoints(int points)
    {
        skillPoints += points;
        Debug.Log("Gained skill points: " + points);
    }

    // Method to take damage (used by enemies or environmental hazards)
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(); // Handle player death
        }
    }

    private void Die()
    {
        // Placeholder death handling
        Debug.Log("Player died!");
        // Add further death logic here, like triggering a respawn or game over.
    }
}
