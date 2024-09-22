using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    // Player Stats
    public float speed = 5f;
    public int health = 100;
    public int maxHealth = 100;
    public int attackPower = 10;
    public int skillPoints = 0;
    public int level = 1;
    public int experiencePoints = 0;
    public int experienceThreshold = 100;

    // Stamina Settings
    public float maxStamina = 100f;
    public float stamina;
    public float staminaRegenRate = 10f;  // Stamina regen per second
    public float staminaDrainRate = 20f;  // Stamina drain per second while running
    public bool isRunning = false;

    // Upgrade costs
    public int speedUpgradeCost = 1;
    public int healthUpgradeCost = 1;
    public int attackUpgradeCost = 1;

    private PlayerControls playerControls;
    private InputAction upgradeAction;
    private UIManager uiManager; // Référence au UIManager pour l'UI
    private CharacterController characterController;

    private void Awake()
    {
        playerControls = new PlayerControls();
        upgradeAction = playerControls.Player.Upgrade;
        characterController = GetComponent<CharacterController>();

        stamina = maxStamina; // Initialiser la stamina à son maximum

        // Bind input actions
        playerControls.Player.Sprint.started += ctx => StartRunning();
        playerControls.Player.Sprint.canceled += ctx => StopRunning();

        if (upgradeAction != null)
        {
            upgradeAction.performed += ctx => UpgradeStat(StatType.Speed, 1f);
        }
        else
        {
            Debug.LogError("L'action 'Upgrade' n'a pas été trouvée dans PlayerControls.");
        }
    }

    private void Start()
    {
        // Récupérer le UIManager pour mettre à jour l'UI
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            UpdateUI();
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        HandleStamina();
        UpdateUI(); // Mettre à jour l'UI en temps réel
    }

    // Gérer la stamina (diminution et régénération)
    private void HandleStamina()
    {
        if (isRunning && stamina > 0)
        {
            // Réduire la stamina pendant que le joueur court
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);

            // Si la stamina est épuisée, arrêter la course
            if (stamina <= 0)
            {
                StopRunning();
            }
        }
        else
        {
            // Régénérer la stamina lorsque le joueur ne court pas
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    // Commence à courir (appelé quand le joueur appuie sur le bouton pour courir)
    private void StartRunning()
    {
        if (stamina > 0)
        {
            isRunning = true;
            speed *= 2; // Doubler la vitesse pendant la course
        }
    }

    // Arrête de courir (appelé quand le joueur relâche le bouton pour courir)
    private void StopRunning()
    {
        isRunning = false;
        speed /= 2; // Revenir à la vitesse normale
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
            maxHealth += amount;
            skillPoints -= healthUpgradeCost;
            healthUpgradeCost++;
            Debug.Log($"Health upgraded by {amount}. New max health: {maxHealth}. Upgrade cost: {healthUpgradeCost}");
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

    // Method to gain experience and skill points
    public void GainExperience(int xp)
    {
        experiencePoints += xp;
        if (experiencePoints >= experienceThreshold)
        {
            LevelUp();
        }
        UpdateUI();
    }

    // Méthode pour augmenter le niveau
    private void LevelUp()
    {
        level++;
        experiencePoints -= experienceThreshold;
        experienceThreshold += 50; // Augmentation de la quantité d'XP requise pour le niveau suivant
        skillPoints += 2;  // Attribuer des points de compétence au niveau supérieur

        Debug.Log($"Niveau supérieur! Niveau actuel: {level}, XP: {experiencePoints}/{experienceThreshold}, Points de compétence: {skillPoints}");
        UpdateUI();
    }

    // Mise à jour de l'UI avec les nouvelles stats
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealth(health, maxHealth);
            uiManager.UpdateExperience(experiencePoints, experienceThreshold);
            uiManager.UpdateLevel(level);
            uiManager.UpdateStamina(stamina, maxStamina); // Mise à jour de la barre de stamina
        }
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
        UpdateUI(); // Update UI after taking damage
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
        PlayerPrefs.SetFloat("Stamina", stamina);
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
        stamina = PlayerPrefs.GetFloat("Stamina", maxStamina); // Charger la stamina
        Debug.Log("Player stats loaded.");
    }
}
