using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCamera : MonoBehaviour
{
    public Transform player;  // Cible à suivre
    public Vector3 offset = new Vector3(0, 10, 0);  // Position de la caméra au-dessus du joueur
    public float zoomSpeed = 2f;
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

    private void OnZoom(InputAction.CallbackContext context)
    {
        float zoomInput = context.ReadValue<float>();
        currentZoom = Mathf.Clamp(currentZoom - zoomInput * zoomSpeed, minZoom, maxZoom);
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = player.position + offset * currentZoom;
        transform.position = desiredPosition;
        transform.LookAt(player.position);  // Toujours regarder le joueur
    }
}
