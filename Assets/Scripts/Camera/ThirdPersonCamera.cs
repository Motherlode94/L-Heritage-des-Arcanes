using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset; // Static offset to maintain distance
    public float smoothSpeed = 0.15f;
    public float rotationSpeed = 80f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;

    private PlayerControls playerControls;
    private float currentZoom = 5f;
    private float targetZoom;
    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private Vector2 lookInput;
    private Vector3 velocity = Vector3.zero; // SmoothDamp velocity vector

    private void Awake()
    {
        // Initialize the offset and input system
        offset = new Vector3(0, 3f, -5f); // Make sure offset is constant
        playerControls = new PlayerControls();
        targetZoom = currentZoom;
    }

    private void OnEnable()
    {
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        playerControls.Player.Zoom.performed += OnZoom;
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        HandleZoom();
        UpdateCameraPosition();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float zoomChange = context.ReadValue<float>();
        targetZoom = Mathf.Clamp(currentZoom - zoomChange * zoomSpeed, minZoom, maxZoom);
    }

    private void HandleCameraRotation()
    {
        // Adjust yaw and pitch based on input
        currentYaw += lookInput.x * rotationSpeed * Time.deltaTime;
        currentPitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -30f, 60f);  // Limit vertical rotation
    }

    private void HandleZoom()
    {
        // Smoothly interpolate zoom value
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSpeed);
    }

    private void UpdateCameraPosition()
    {
        // Adjust camera position smoothly based on yaw, pitch, and zoom
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 direction = rotation * Vector3.forward;

        // Calculate desired position using the offset and zoom
        Vector3 desiredPosition = player.position - direction * currentZoom + offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // Make sure the camera looks at the player
        transform.LookAt(player.position + Vector3.up * 1.5f);  // Slightly above the player
    }

    // Implement a method for camera transitions
    public void StartCameraTransition(Transform newCameraPosition, float duration)
    {
        StartCoroutine(CameraTransition(newCameraPosition, duration));
    }

    private IEnumerator CameraTransition(Transform newCameraPosition, float duration)
    {
        // Store starting position and rotation
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Smoothly interpolate position and rotation using Lerp and Slerp
            transform.position = Vector3.Lerp(startPosition, newCameraPosition.position, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(startRotation, newCameraPosition.rotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera reaches the final position and rotation
        transform.position = newCameraPosition.position;
        transform.rotation = newCameraPosition.rotation;
    }
}
