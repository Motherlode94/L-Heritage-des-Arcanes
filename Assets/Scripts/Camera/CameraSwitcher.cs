using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public Camera firstPersonCamera;   // Caméra pour la vue à la première personne
    public Camera thirdPersonCamera;   // Caméra pour la vue à la troisième personne
    public Camera topDownCamera;       // Caméra pour la vue d'aigle

    public float transitionDuration = 1.5f;  // Durée de la transition douce
    public AnimationCurve transitionCurve;   // Pour ajuster la courbe de transition

    private PlayerControls playerControls;   // Système d'input

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
        SetThirdPersonView();  // Par défaut, on commence en vue à la troisième personne
    }

    // Appelé lorsqu'on appuie sur le bouton pour changer de vue
    private void OnSwitchCameraView(InputAction.CallbackContext context)
    {
        if (thirdPersonCamera.enabled)
        {
            StartCoroutine(SmoothTransition(thirdPersonCamera, firstPersonCamera, transitionDuration));
        }
        else if (firstPersonCamera.enabled)
        {
            StartCoroutine(SmoothTransition(firstPersonCamera, topDownCamera, transitionDuration));
        }
        else if (topDownCamera.enabled)
        {
            StartCoroutine(SmoothTransition(topDownCamera, thirdPersonCamera, transitionDuration));
        }
    }

    // Transition douce entre deux caméras
    private IEnumerator SmoothTransition(Camera fromCamera, Camera toCamera, float duration)
    {
        float elapsedTime = 0f;

        // Capturer la position, rotation et FOV de départ
        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;
        float startFOV = fromCamera.fieldOfView;

        // Capture des valeurs de la caméra cible
        Vector3 endPosition = toCamera.transform.position;
        Quaternion endRotation = toCamera.transform.rotation;
        float endFOV = toCamera.fieldOfView;

        toCamera.enabled = true;  // Activer la caméra cible dès le début de la transition
        toCamera.fieldOfView = startFOV;  // Synchroniser les FOV pour une transition fluide

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = transitionCurve.Evaluate(t);  // Utiliser une courbe pour lisser la transition

            // Interpolation des positions, rotations et champ de vision
            toCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            toCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            toCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);

            yield return null;  // Attendre une frame
        }

        fromCamera.enabled = false;  // Désactiver la caméra source à la fin de la transition
    }

    // Méthode pour passer directement en vue à la première personne
    private void SetFirstPersonView()
    {
        firstPersonCamera.enabled = true;
        thirdPersonCamera.enabled = false;
        topDownCamera.enabled = false;
    }

    // Méthode pour passer directement en vue à la troisième personne
    private void SetThirdPersonView()
    {
        firstPersonCamera.enabled = false;
        thirdPersonCamera.enabled = true;
        topDownCamera.enabled = false;
    }

    // Méthode pour passer directement en vue d'aigle
    private void SetTopDownView()
    {
        firstPersonCamera.enabled = false;
        thirdPersonCamera.enabled = false;
        topDownCamera.enabled = true;
    }
}
