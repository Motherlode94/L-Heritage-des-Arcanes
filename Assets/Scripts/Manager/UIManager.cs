using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this line

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject healthPanel;
    public GameObject manaPanel;
    public GameObject staminaPanel;
    public GameObject dialoguePanel;
    public GameObject pauseMenuPanel;

    [Header("Health UI Elements")]
    public Slider healthBar;   // Health bar UI (slider)
    
    [Header("Mana UI Elements")]
    public Slider manaBar;     // Mana bar UI (slider)

    [Header("Stamina UI Elements")]
    public Slider staminaBar;  // Stamina bar UI (slider)

    [Header("Experience & Level UI Elements")]
    public TextMeshProUGUI levelText;  // Changed to TextMeshProUGUI
    public Slider experienceBar;  // Barre pour afficher l'expérience
    public TextMeshProUGUI experienceText;  // Changed to TextMeshProUGUI

    [Header("Dialogue UI Elements")]
    public TextMeshProUGUI dialogueText; // Changed to TextMeshProUGUI
    public Button[] dialogueOptions; // Button options for player dialogue choices

    private InventoryUI inventoryUI;

    void Start()
    {
        // Initialize Inventory UI if available
        if (inventoryPanel != null)
        {
            inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
        }

        // Activer les panneaux de santé, mana, et stamina
        ToggleHealthPanel(true);
        ToggleManaPanel(true);
        ToggleStaminaPanel(true);

        // Optionally hide other panels at the start
        HideAllOptionalPanels();
    }

    // Toggle the visibility of the inventory panel
    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isActive = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isActive);
            if (!isActive && inventoryUI != null)
            {
                inventoryUI.UpdateInventoryUI(); // Update inventory UI when it's opened
            }
        }
    }

    // Show or hide the health panel
    public void ToggleHealthPanel(bool show)
    {
        if (healthPanel != null)
        {
            healthPanel.SetActive(show);
        }
    }

    // Show or hide the mana panel
    public void ToggleManaPanel(bool show)
    {
        if (manaPanel != null)
        {
            manaPanel.SetActive(show);
        }
    }

    // Show or hide the stamina panel
    public void ToggleStaminaPanel(bool show)
    {
        if (staminaPanel != null)
        {
            staminaPanel.SetActive(show);
        }
    }

    // Update the player's health bar
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // Update the player's mana bar
    public void UpdateMana(float currentMana, float maxMana)
    {
        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = currentMana;
        }
    }

    // Update the player's stamina bar
    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
    }

    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }

    // Mise à jour de l'expérience du joueur
    public void UpdateExperience(int currentXP, int maxXP)
    {
        if (experienceBar != null)
        {
            experienceBar.maxValue = maxXP;
            experienceBar.value = currentXP;
        }
        if (experienceText != null)
        {
            experienceText.text = $"XP: {currentXP}/{maxXP}";
        }
    }

    // Show a dialogue panel and update its text
    public void ShowDialogue(string dialogue, string[] options = null)
    {
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = dialogue;

            // Show dialogue options if available
            if (options != null && dialogueOptions != null)
            {
                for (int i = 0; i < dialogueOptions.Length; i++)
                {
                    if (i < options.Length)
                    {
                        dialogueOptions[i].gameObject.SetActive(true);
                        dialogueOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i]; // Changed to TextMeshProUGUI
                    }
                    else
                    {
                        dialogueOptions[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // Hide the dialogue panel
    public void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    // Show or hide the pause menu
    public void TogglePauseMenu(bool show)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(show);
            Time.timeScale = show ? 0 : 1;  // Pause or resume the game
        }
    }

    // Hide optional panels like inventory, dialogue, etc.
    public void HideAllOptionalPanels()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
}
