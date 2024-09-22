using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Victory UI Elements")]
    public GameObject victoryMessage;         // Le message de victoire
    public Button nextLevelButton;            // Le bouton pour passer au niveau suivant
    public float fadeDuration = 1.5f;         // Durée du fondu du message de victoire
    public float restartDelay = 3f;           // Délai avant de redémarrer automatiquement
    private CanvasGroup victoryCanvasGroup;

    [Header("Player Stats")]
    public float playerHealth = 100f;         // Vie du joueur
    public float maxHealth = 100f;
    public float playerMana = 50f;            // Mana du joueur
    public float maxMana = 50f;
    public float playerStamina = 75f;         // Stamina du joueur
    public float maxStamina = 75f;

    private int totalEnemies;                 // Nombre total d'ennemis
    private UIManager uiManager;              // Référence au UIManager pour mettre à jour les barres de vie/mana/stamina

    private PlayerControls playerControls;    // Référence au nouveau système d'Input

    void Awake()
    {
        // Initialiser les contrôles du joueur
        playerControls = new PlayerControls();

        // Associer les actions d'input à des méthodes
        playerControls.Player.UseMana.performed += ctx => UseMana(10f);
        playerControls.Player.UseStamina.performed += ctx => UseStamina(10f);
        playerControls.Player.RestoreHealth.performed += ctx => RestoreHealth(20f);
    }

    void OnEnable()
    {
        playerControls.Enable(); // Activer les inputs quand le script est actif
    }

    void OnDisable()
    {
        playerControls.Disable(); // Désactiver les inputs quand le script est désactivé
    }

    void Start()
    {
        // Initialiser le nombre total d'ennemis
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // Initialiser CanvasGroup pour l'animation du message de victoire
        victoryCanvasGroup = victoryMessage.GetComponent<CanvasGroup>();
        if (victoryCanvasGroup != null)
        {
            victoryCanvasGroup.alpha = 0;
            victoryMessage.SetActive(false);
        }

        // Désactiver le bouton au départ
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }

        // Obtenir le UIManager pour mettre à jour les barres d'état du joueur
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            UpdateUI();
        }
    }

    public void EnemyKilled()
    {
        totalEnemies--;

        // Si tous les ennemis sont tués, déclencher la victoire
        if (totalEnemies <= 0)
        {
            Victory();
        }
    }

    void Victory()
    {
        if (victoryCanvasGroup != null)
        {
            StartCoroutine(FadeInVictoryMessage());
            StartCoroutine(RestartGameAfterDelay());
        }
        else
        {
            victoryMessage.SetActive(true);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
        }
    }

    // Fonction pour infliger des dégâts au joueur
    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            GameOver();
        }
        UpdateUI();
    }

    // Fonction pour restaurer la santé, la mana ou la stamina
    public void RestoreHealth(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxHealth);
        UpdateUI();
    }

    public void UseMana(float amount)
    {
        playerMana = Mathf.Clamp(playerMana - amount, 0, maxMana);
        UpdateUI();
    }

    public void RestoreMana(float amount)
    {
        playerMana = Mathf.Clamp(playerMana + amount, 0, maxMana);
        UpdateUI();
    }

    public void UseStamina(float amount)
    {
        playerStamina = Mathf.Clamp(playerStamina - amount, 0, maxStamina);
        UpdateUI();
    }

    public void RestoreStamina(float amount)
    {
        playerStamina = Mathf.Clamp(playerStamina + amount, 0, maxStamina);
        UpdateUI();
    }

    // Mettre à jour les barres de vie, mana, stamina
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealth(playerHealth, maxHealth);
            uiManager.UpdateMana(playerMana, maxMana);
            uiManager.UpdateStamina(playerStamina, maxStamina);
        }
    }

    // Fonction de Game Over si le joueur meurt
    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Coroutine pour faire un fondu progressif du message de victoire
    private IEnumerator FadeInVictoryMessage()
    {
        float elapsedTime = 0f;
        victoryMessage.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            victoryCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        victoryCanvasGroup.alpha = 1f;
    }

    // Coroutine pour redémarrer le jeu après un délai
    private IEnumerator RestartGameAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Fonction pour charger le niveau suivant
    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
