using System.Collections; // Import System.Collections for IEnumerator
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 3, -5);
    public float smoothSpeed = 0.125f;
    public float rotationSpeed = 2f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    public float collisionOffset = 0.2f;

    private Vector2 rotationInput;
    private float currentZoom = 5f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private PlayerControls playerControls;

    // Method to start camera transition
    public void StartCameraTransition(Transform newTargetPosition, float duration)
    {
        StartCoroutine(CameraTransition(newTargetPosition, duration));
    }

    private IEnumerator CameraTransition(Transform newTargetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, newTargetPosition.position, timeElapsed / duration);
            transform.rotation = Quaternion.Slerp(startRotation, newTargetPosition.rotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position and rotation
        transform.position = newTargetPosition.position;
        transform.rotation = newTargetPosition.rotation;
    }

    private void Awake()
    {
        playerControls = new PlayerControls(); // Initialize input controls
    }

    private void OnEnable()
    {
        playerControls.Player.Look.performed += OnLook;  // Register camera rotation (look) action
        playerControls.Player.Look.canceled += OnLookCanceled;  // Reset rotation input when canceled
        playerControls.Player.Zoom.performed += OnZoom;  // Register zoom action
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rotationInput = context.ReadValue<Vector2>();
        }
    }

    public void OnLookCanceled(InputAction.CallbackContext context)
    {
        rotationInput = Vector2.zero;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float zoomChange = context.ReadValue<float>();
            currentZoom = Mathf.Clamp(currentZoom - zoomChange * zoomSpeed, minZoom, maxZoom);
        }
    }

    private void LateUpdate()
    {
        HandleRotation();
        HandleZoom();
        HandleCollision();
        UpdateCameraPosition();
    }

    private void HandleRotation()
    {
        currentYaw += rotationInput.x * rotationSpeed * Time.deltaTime;
        currentPitch -= rotationInput.y * rotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -30f, 60f);
    }

    private void HandleZoom()
    {
        // The zooming is now handled by OnZoom(), adjusting the 'currentZoom' value
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 desiredPosition = player.position - rotation * Vector3.forward * currentZoom + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(player.position + Vector3.up * 2f);
    }

    private void HandleCollision()
    {
        RaycastHit hit;
        Vector3 direction = (transform.position - player.position).normalized;

        if (Physics.Raycast(player.position, direction, out hit, currentZoom))
        {
            float adjustedZoom = Mathf.Clamp(hit.distance - collisionOffset, minZoom, maxZoom);
            currentZoom = Mathf.Lerp(currentZoom, adjustedZoom, Time.deltaTime * zoomSpeed);
        }
    }
}
