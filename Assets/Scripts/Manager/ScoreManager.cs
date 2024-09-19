using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int comboMultiplier = 1;
    public float comboResetTime = 5f;
    private float comboTimer;

    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            ResetCombo();
        }
    }

    public void AddScore(int points)
    {
        score += points * comboMultiplier;
        Debug.Log("Score: " + score);
        comboTimer = comboResetTime; // Reset combo timer
    }

    public void IncreaseCombo()
    {
        comboMultiplier++;
        comboTimer = comboResetTime; // Reset timer for combo
        // Vous pouvez ajouter un effet visuel ici (ex : afficher le multiplicateur à l'écran)
    }

    public void ResetCombo()
    {
        comboMultiplier = 1;
        Debug.Log("Combo reset.");
    }

    // Vous pouvez aussi ajouter un système de rétroaction sonore ici
    // public AudioSource comboSound; -> Jouez un son lors de l'augmentation du combo
}
