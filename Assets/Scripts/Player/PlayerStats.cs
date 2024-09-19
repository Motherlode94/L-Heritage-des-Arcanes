using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public float speed = 5f;
    public int health = 100;
    public int attackPower = 10;
    public int skillPoints = 0; // Skill points available for upgrades
    public int level = 1;
    public int experiencePoints = 0;
    public int experienceThreshold = 100; // XP required to level up

    public int speedUpgradeCost = 1;
    public int healthUpgradeCost = 1;
    public int attackUpgradeCost = 1;

    private PlayerControls playerControls;
    private InputAction upgradeAction;

    private void Awake()
    {
        playerControls = new PlayerControls();
        upgradeAction = playerControls.Player.Upgrade; // Find the "Upgrade" action in the Player input map

        if (upgradeAction != null)
        {
            upgradeAction.performed += ctx => UpgradeStat(StatType.Speed, 1f); // Example: Upgrade speed by default
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

    // Enum for stat types
    public enum StatType { Speed, Health, Attack }

    // Method to upgrade different stats
    public void UpgradeStat(StatType statType, float amount)
    {
        if (skillPoints > 0)
        {
            switch (statType)
            {
                case StatType.Speed:
                    UpgradeSpeed(amount);
                    break;
                case StatType.Health:
                    UpgradeHealth((int)amount);
                    break;
                case StatType.Attack:
                    UpgradeAttack((int)amount);
                    break;
            }
            skillPoints--; // Deduct a skill point
            Debug.Log($"{statType} upgraded by {amount}. Remaining skill points: {skillPoints}");
        }
        else
        {
            Debug.Log("Not enough skill points!");
        }
    }

    // Methods to upgrade stats
    public void UpgradeSpeed(float amount)
    {
        if (skillPoints >= speedUpgradeCost)
        {
            speed += amount;
            skillPoints -= speedUpgradeCost;
            speedUpgradeCost++; // Cost increases with each upgrade
            Debug.Log($"Speed upgraded by {amount}. New speed: {speed}. Upgrade cost: {speedUpgradeCost}");
        }
        else
        {
            Debug.Log("Not enough skill points for speed upgrade!");
        }
    }

    public void UpgradeHealth(int amount)
    {
        if (skillPoints >= healthUpgradeCost)
        {
            health += amount;
            skillPoints -= healthUpgradeCost;
            healthUpgradeCost++;
            Debug.Log($"Health upgraded by {amount}. New health: {health}. Upgrade cost: {healthUpgradeCost}");
        }
        else
        {
            Debug.Log("Not enough skill points for health upgrade!");
        }
    }

    public void UpgradeAttack(int amount)
    {
        if (skillPoints >= attackUpgradeCost)
        {
            attackPower += amount;
            skillPoints -= attackUpgradeCost;
            attackUpgradeCost++;
            Debug.Log($"Attack power upgraded by {amount}. New attack power: {attackPower}. Upgrade cost: {attackUpgradeCost}");
        }
        else
        {
            Debug.Log("Not enough skill points for attack upgrade!");
        }
    }

    // Method to gain skill points and experience
    public void EarnSkillPoints(int points)
    {
        skillPoints += points;
        Debug.Log($"Gained skill points: {points}. Total skill points: {skillPoints}");
    }

    public void GainExperience(int xp)
    {
        experiencePoints += xp;
        if (experiencePoints >= experienceThreshold)
        {
            LevelUp();
        }
        Debug.Log($"Gained {xp} XP. Total XP: {experiencePoints}/{experienceThreshold}");
    }

    private void LevelUp()
    {
        level++;
        experiencePoints -= experienceThreshold;
        experienceThreshold += 50; // Increase the XP threshold for the next level
        skillPoints += 2; // Award skill points for leveling up
        Debug.Log($"Level up! Current level: {level}. Skill points available: {skillPoints}");
    }

    // Method to take damage (used by enemies or environmental hazards)
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(); // Handle player death
        }
        Debug.Log($"Took {damage} damage. Current health: {health}");
    }

    private void Die()
    {
        // Placeholder death handling
        Debug.Log("Player died!");
        // Add further death logic here, like triggering a respawn or game over.
    }

    // Save and load system using PlayerPrefs
    public void SaveStats()
    {
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetFloat("Speed", speed);
        PlayerPrefs.SetInt("AttackPower", attackPower);
        PlayerPrefs.SetInt("SkillPoints", skillPoints);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("ExperiencePoints", experiencePoints);
        PlayerPrefs.SetInt("ExperienceThreshold", experienceThreshold);
        PlayerPrefs.Save();
        Debug.Log("Player stats saved.");
    }

    public void LoadStats()
    {
        health = PlayerPrefs.GetInt("Health", 100); // Default value is 100 if no saved value
        speed = PlayerPrefs.GetFloat("Speed", 5f);  // Default value is 5f
        attackPower = PlayerPrefs.GetInt("AttackPower", 10);  // Default is 10
        skillPoints = PlayerPrefs.GetInt("SkillPoints", 0);   // Default is 0
        level = PlayerPrefs.GetInt("Level", 1);               // Default is 1
        experiencePoints = PlayerPrefs.GetInt("ExperiencePoints", 0); // Default is 0
        experienceThreshold = PlayerPrefs.GetInt("ExperienceThreshold", 100); // Default is 100
        Debug.Log("Player stats loaded.");
    }
}
