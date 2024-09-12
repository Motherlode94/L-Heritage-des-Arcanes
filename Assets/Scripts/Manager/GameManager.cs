using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject victoryMessage;         // Le message de victoire
    public Button nextLevelButton;            // Le bouton pour passer au niveau suivant
    public float fadeDuration = 1.5f;         // Durée du fondu du message de victoire
    public float restartDelay = 3f;           // Délai avant de redémarrer automatiquement (si utilisé)
    private int totalEnemies;                 // Nombre total d'ennemis
    private CanvasGroup victoryCanvasGroup;

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

            // Réinitialiser automatiquement après un délai (optionnel)
            StartCoroutine(RestartGameAfterDelay());
        }
        else
        {
            victoryMessage.SetActive(true);
        }

        // Activer le bouton de niveau suivant
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
        }
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

        // Vous pouvez utiliser SceneManager pour charger le même niveau ou un autre
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Fonction pour charger le niveau suivant
    private void LoadNextLevel()
    {
        // Charger le prochain niveau (assurez-vous d'avoir configuré les niveaux dans le build settings)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
