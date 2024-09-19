using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public Transform thirdPersonTarget;
    public Transform firstPersonTarget;
    public Transform topDownTarget;

    public Camera mainCamera;
    public float smoothSpeed = 0.1f;
    public float transitionDuration = 1.5f;

    private Transform currentTarget;
    private bool isTransitioning = false;
    private float transitionProgress;

    private PlayerControls playerControls;

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
        SetThirdPersonView();  // Définir la caméra par défaut
    }

    private void Update()
    {
        if (isTransitioning)
        {
            PerformTransition();
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
}
