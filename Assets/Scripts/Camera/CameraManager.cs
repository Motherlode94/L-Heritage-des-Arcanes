using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public Transform firstPersonTarget;  // Position et rotation cible pour la caméra First Person
    public Transform thirdPersonTarget;  // Position et rotation cible pour la caméra Third Person
    public Transform topDownTarget;      // Position et rotation cible pour la vue d'aigle

    public Camera mainCamera;            // La caméra principale qui va effectuer les transitions
    public float transitionDuration = 1.5f; // Durée de la transition en secondes

    private bool isTransitioning = false;
    private Transform currentTarget;
    private float transitionProgress;

    private PlayerControls playerControls; // Système d'input

    private void Awake()
    {
        playerControls = new PlayerControls(); // Initialiser les contrôles

        // Relier l'action SwitchCameraView à la fonction OnSwitchCameraView
        playerControls.Player.SwitchCameraView.performed += OnSwitchCameraView;
    }

    private void OnEnable()
    {
        playerControls.Enable(); // Activer les inputs
    }

    private void OnDisable()
    {
        playerControls.Disable(); // Désactiver les inputs
    }

    private void Start()
    {
        // Par défaut, activer la vue à la troisième personne
        SetThirdPersonView();
    }

    // Fonction déclenchée lorsque la touche pour changer la vue de la caméra est appuyée
    private void OnSwitchCameraView(InputAction.CallbackContext context)
    {
        if (context.performed && !isTransitioning)
        {
            // Alterner entre les vues de caméra
            if (currentTarget == firstPersonTarget)
            {
                SetThirdPersonView();
            }
            else if (currentTarget == thirdPersonTarget)
            {
                SetTopDownView();
            }
            else if (currentTarget == topDownTarget)
            {
                SetFirstPersonView();
            }
        }
    }

    // Définir la vue à la première personne
    private void SetFirstPersonView()
    {
        StartTransition(firstPersonTarget);
    }

    // Définir la vue à la troisième personne
    private void SetThirdPersonView()
    {
        StartTransition(thirdPersonTarget);
    }

    // Définir la vue en hauteur (vue d'aigle)
    private void SetTopDownView()
    {
        StartTransition(topDownTarget);
    }

    // Démarrer la transition vers une nouvelle cible de caméra
    private void StartTransition(Transform newTarget)
    {
        currentTarget = newTarget; // Changer la cible de la caméra
        transitionProgress = 0f;   // Réinitialiser la progression de la transition
        isTransitioning = true;    // Marquer que la transition est en cours
    }

    private void Update()
    {
        if (isTransitioning)
        {
            PerformTransition(); // Effectuer la transition de caméra
        }
    }

    // Effectuer la transition entre la position actuelle et la nouvelle cible
    private void PerformTransition()
    {
        transitionProgress += Time.deltaTime / transitionDuration;

        // Interpolation de la position et de la rotation de la caméra
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, currentTarget.position, transitionProgress);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, currentTarget.rotation, transitionProgress);

        // Lorsque la transition est terminée
        if (transitionProgress >= 1f)
        {
            isTransitioning = false; // Fin de la transition
        }
    }
}
