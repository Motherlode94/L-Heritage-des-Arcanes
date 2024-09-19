using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;  // Référence au joueur
    public float mouseSensitivity = 100f;
    public Transform playerBody;  // Le corps du joueur pour tourner

    private float xRotation = 0f;
    private PlayerControls playerControls;
    private Vector2 lookInput;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Verrouiller le curseur
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limiter la rotation verticale

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // Rotation verticale
        playerBody.Rotate(Vector3.up * mouseX);  // Rotation horizontale avec le corps
    }
}
