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
    }

    public void ResetCombo()
    {
        comboMultiplier = 1;
    }
}
