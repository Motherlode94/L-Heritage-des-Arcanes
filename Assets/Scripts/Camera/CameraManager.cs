using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public Camera thirdPersonCamera;
    public Camera topDownCamera;
    public float transitionSpeed = 1.5f; // Temps du fondu
    public Image fadeImage; // L'image de fondu dans le Canvas
    public AnimationCurve fadeCurve; // Courbe de transition pour le fondu

    private PlayerControls playerControls;
    private bool isTransitioning = false;
    private bool topDownUnlocked = false;  // Bonus de la caméra Top-Down

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.SwitchCameraView.performed += OnSwitchCameraView;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        SetThirdPersonView();  // Vue par défaut à la troisième personne
    }

    private void OnSwitchCameraView(InputAction.CallbackContext context)
    {
        if (!isTransitioning && topDownUnlocked) // La vue Top-Down n'est accessible que si débloquée
        {
            if (thirdPersonCamera.enabled)
            {
                StartCoroutine(TransitionCamera(topDownCamera));
            }
            else if (topDownCamera.enabled)
            {
                StartCoroutine(TransitionCamera(thirdPersonCamera));
            }
        }
    }

    private IEnumerator TransitionCamera(Camera targetCamera)
    {
        isTransitioning = true;

        // Début du fondu-out (apparition de l'image noire)
        yield return StartCoroutine(FadeOut());

        // Désactiver toutes les caméras au début de la transition
        thirdPersonCamera.enabled = false;
        topDownCamera.enabled = false;

        // Activer la nouvelle caméra cible après une petite pause
        targetCamera.enabled = true;

        // Début du fondu-in (disparition de l'image noire)
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        while (time < transitionSpeed)
        {
            float alpha = fadeCurve.Evaluate(time / transitionSpeed);
            SetFadeAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }
        SetFadeAlpha(1); // Complètement noir
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < transitionSpeed)
        {
            float alpha = fadeCurve.Evaluate(1 - (time / transitionSpeed));
            SetFadeAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }
        SetFadeAlpha(0); // Complètement transparent
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    // Méthode pour activer la vue à la troisième personne
    private void SetThirdPersonView()
    {
        thirdPersonCamera.enabled = true;
        topDownCamera.enabled = false;
    }

    // Méthode pour activer la vue top-down une fois débloquée
    public void UnlockTopDownView()
    {
        topDownUnlocked = true;
    }
}
