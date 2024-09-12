using UnityEngine;
using System.Collections;

public class SlowMotionEffect : MonoBehaviour
{
    public float slowMotionFactor = 0.2f; // Facteur de ralentissement
    public float slowDownTransitionSpeed = 0.5f; // Vitesse de transition vers le ralenti
    public float restoreTransitionSpeed = 1.5f; // Vitesse de transition pour revenir à la normale

    private bool isSlowMotionActive = false;

    public void TriggerSlowMotion(float duration)
    {
        if (!isSlowMotionActive)
        {
            StartCoroutine(SlowDown(duration));
        }
    }

    private IEnumerator SlowDown(float duration)
    {
        isSlowMotionActive = true;

        // Transition fluide vers le ralenti
        float currentSpeed = Time.timeScale;
        while (currentSpeed > slowMotionFactor)
        {
            currentSpeed -= Time.unscaledDeltaTime / slowDownTransitionSpeed;
            Time.timeScale = Mathf.Clamp(currentSpeed, slowMotionFactor, 1f);
            yield return null;
        }

        // Maintenir le ralenti pendant la durée spécifiée
        yield return new WaitForSecondsRealtime(duration);

        // Transition fluide pour revenir à la vitesse normale
        while (Time.timeScale < 1f)
        {
            Time.timeScale += Time.unscaledDeltaTime / restoreTransitionSpeed;
            Time.timeScale = Mathf.Clamp(Time.timeScale, slowMotionFactor, 1f);
            yield return null;
        }

        // Rétablir le temps à la normale
        Time.timeScale = 1f;
        isSlowMotionActive = false;
    }
}
