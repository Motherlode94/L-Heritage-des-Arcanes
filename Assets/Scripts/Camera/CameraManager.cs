using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public Transform thirdPersonTarget;
    public Transform firstPersonTarget;
    public Transform topDownTarget;

    public Camera mainCamera;
    public float smoothSpeed = 0.1f;
    public float transitionSpeed = 5f; // Vitesse de transition entre les caméras
    public float firstPersonFOV = 60f;
    public float thirdPersonFOV = 70f;
    public float topDownFOV = 90f;

    private Transform currentTarget;
    private bool isTransitioning = false;
    private PlayerControls playerControls;
    private Transform playerHead;
    private float targetFOV;

    private Vector3 velocity = Vector3.zero; // SmoothDamp velocity reference
    private float fovVelocity = 0f; // SmoothDamp for FOV

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.SwitchCameraView.performed += OnSwitchCameraView;

        // Find player head for first-person view
        playerHead = GameObject.FindWithTag("PlayerHead").transform;
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
        SetThirdPersonView();  // Default camera view
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
        StartTransition(firstPersonTarget, firstPersonFOV);

        if (playerHead != null)
        {
            mainCamera.transform.position = playerHead.position;
            mainCamera.transform.rotation = playerHead.rotation;
        }
    }

    private void SetThirdPersonView()
    {
        StartTransition(thirdPersonTarget, thirdPersonFOV);
    }

    private void SetTopDownView()
    {
        StartTransition(topDownTarget, topDownFOV);
    }

    private void StartTransition(Transform newTarget, float newFOV)
    {
        currentTarget = newTarget;
        targetFOV = newFOV;
        isTransitioning = true;
    }

    private void PerformTransition()
    {
        // SmoothDamp for position
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, currentTarget.position, ref velocity, smoothSpeed);

        // Slerp for rotation
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, currentTarget.rotation, Time.deltaTime * transitionSpeed);

        // SmoothDamp for FOV
        mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, targetFOV, ref fovVelocity, smoothSpeed);

        // Stop transition if close enough to the target position/rotation
        if (Vector3.Distance(mainCamera.transform.position, currentTarget.position) < 0.01f &&
            Quaternion.Angle(mainCamera.transform.rotation, currentTarget.rotation) < 0.1f &&
            Mathf.Abs(mainCamera.fieldOfView - targetFOV) < 0.1f)
        {
            isTransitioning = false;
            mainCamera.transform.position = currentTarget.position;
            mainCamera.transform.rotation = currentTarget.rotation;
            mainCamera.fieldOfView = targetFOV;
        }
    }
}
