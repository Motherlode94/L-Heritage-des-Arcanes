using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCamera : MonoBehaviour
{
    public Transform player;
    public float height = 10f;
    public float smoothSpeed = 0.125f;
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    private float currentZoom = 10f;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Zoom.performed += OnZoom;
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float zoomInput = context.ReadValue<float>();
        currentZoom = Mathf.Clamp(currentZoom - zoomInput * zoomSpeed, minZoom, maxZoom);
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = player.position + Vector3.up * currentZoom;  // Follow the player from above
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(player.position);  // Always look at the player
    }
}
