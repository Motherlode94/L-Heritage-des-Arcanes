using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public Transform thirdPersonTarget;
    public Transform firstPersonTarget;
    public Transform topDownTarget;

    public Camera mainCamera;
    public float smoothSpeed = 0.1f;
    public float baseHeadMovementSpeed = 50f;
    public float headRotationLimit = 60f;
    public float transitionDuration = 1.5f;

    public float minRotationSpeed = 20f;  // Vitesse minimale de rotation quand le joueur est immobile
    public float maxRotationSpeed = 100f; // Vitesse maximale de rotation quand le joueur court

    private Vector2 lookInput;
    private Transform currentTarget;
    private float transitionProgress;
    private bool isTransitioning = false;
    private PlayerControls playerControls;
    private bool isHeadTurning = false;

    private Quaternion targetRotation;
    public float rotationSmoothTime = 0.1f;
    private Quaternion currentRotationVelocity;

    private CharacterController playerController;  // Référence au contrôleur du joueur
    private float currentSpeed;  // Vitesse actuelle du joueur

    // Ajouter une variable pour la vitesse maximale du joueur
    public float maxPlayerSpeed = 10f; // Ex. Vitesse maximale définie par vos scripts de mouvement

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.SwitchCameraView.performed += OnSwitchCameraView;
        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>(); 
        playerControls.Player.Look.canceled += ctx => isHeadTurning = false;

        playerController = FindObjectOfType<CharacterController>(); // Récupérer le CharacterController du joueur
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
        SetThirdPersonView();
        targetRotation = mainCamera.transform.rotation;
    }

    private void Update()
    {
        if (isTransitioning)
        {
            PerformTransition();
        }
        else
        {
            UpdatePlayerSpeed();  // Met à jour la vitesse actuelle du joueur
            if (currentTarget == thirdPersonTarget)
            {
                HandleHeadMovement();
            }

            FollowPlayer();
        }
    }

    private void OnSwitchCameraView(InputAction.CallbackContext context)
    {
        if (!isTransitioning)
        {
            if (currentTarget == thirdPersonTarget)
            {
                SetFirstPersonView();
            }
            else if (currentTarget == firstPersonTarget)
            {
                SetTopDownView();
            }
            else if (currentTarget == topDownTarget)
            {
                SetThirdPersonView();
            }
        }
    }

    private void SetFirstPersonView()
    {
        StartTransition(firstPersonTarget);
    }

    private void SetThirdPersonView()
    {
        StartTransition(thirdPersonTarget);
    }

    private void SetTopDownView()
    {
        StartTransition(topDownTarget);
    }

    private void StartTransition(Transform newTarget)
    {
        currentTarget = newTarget;
        transitionProgress = 0f;
        isTransitioning = true;
    }

    private void PerformTransition()
    {
        transitionProgress += Time.deltaTime / transitionDuration;

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, currentTarget.position, transitionProgress);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, currentTarget.rotation, transitionProgress);

        if (transitionProgress >= 1f)
        {
            isTransitioning = false;
        }
    }

    private void FollowPlayer()
    {
        if (currentTarget == thirdPersonTarget)
        {
            Vector3 desiredPosition = thirdPersonTarget.position;
            Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, desiredPosition, smoothSpeed);
            mainCamera.transform.position = smoothedPosition;

            mainCamera.transform.LookAt(thirdPersonTarget.position + Vector3.up * 1.5f);
        }
    }

    // Mettre à jour la vitesse actuelle du joueur
    private void UpdatePlayerSpeed()
    {
        if (playerController != null)
        {
            currentSpeed = playerController.velocity.magnitude;  // Obtenir la vitesse actuelle du joueur
        }
    }

    // Appliquer le mouvement de tête avec une inertie ajustée en fonction de la vitesse du joueur
    private void HandleHeadMovement()
    {
        if (lookInput != Vector2.zero) // Si on reçoit une entrée
        {
            isHeadTurning = true;

            // Ajuster la vitesse de rotation en fonction de la vitesse du joueur
            float adjustedHeadMovementSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, currentSpeed / maxPlayerSpeed);

            // Rotation horizontale (yaw) et verticale (pitch)
            float yawRotation = lookInput.x * adjustedHeadMovementSpeed * Time.deltaTime;
            float pitchRotation = lookInput.y * adjustedHeadMovementSpeed * Time.deltaTime;

            // Calcul des nouvelles rotations
            float currentYaw = Mathf.Clamp(mainCamera.transform.eulerAngles.y + yawRotation, -headRotationLimit, headRotationLimit);
            float currentPitch = Mathf.Clamp(mainCamera.transform.eulerAngles.x - pitchRotation, -headRotationLimit, headRotationLimit);

            // Définir la rotation cible avec limitation
            targetRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }

        // Appliquer l'inertie de la rotation avec Quaternion.Slerp
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRotation, rotationSmoothTime);
    }
}
