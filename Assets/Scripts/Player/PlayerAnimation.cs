using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Transform playerBody;
    public Transform playerCamera;

    private bool isCrouching = false;
    private bool isJumping = false;
    private bool isSwimming = false;  // Ajout du contrôle de natation

    public float walkSpeed = 2f;
    public float crouchSpeed = 1.5f;
    public float sprintSpeed = 5f;
    public float swimSpeed = 3f;  // Vitesse de natation
    public float mouseSensitivity = 100f;
    public float rotationSmoothTime = 0.1f;
    private float xRotation = 0f;

    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool sprintInput;
    private bool crouchInput;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new PlayerControls();

        // Actions liées au mouvement
        playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Sprint.performed += ctx => sprintInput = true;
        playerControls.Player.Sprint.canceled += ctx => sprintInput = false;
        playerControls.Player.Crouch.performed += ctx => crouchInput = !crouchInput;
    }

    void OnEnable()
    {
        playerControls.Player.Enable();
    }

    void OnDisable()
    {
        playerControls.Player.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCrouch();
        AlignBodyToCamera();
        UpdateAnimationParameters();  // Mise à jour des paramètres du Blend Tree
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        if (isSwimming)
        {
            HandleSwimming(move);
        }
        else
        {
            HandleGroundMovement(move);
        }
    }

    void HandleGroundMovement(Vector3 move)
    {
        float currentSpeed = walkSpeed;

        if (sprintInput && movementInput.y > 0 && !isCrouching)
        {
            currentSpeed = sprintSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleSwimming(Vector3 move)
    {
        characterController.Move(move * swimSpeed * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void AlignBodyToCamera()
    {
        Vector3 cameraForward = playerCamera.forward;
        cameraForward.y = 0;  // Éviter que le joueur se penche ou se lève

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetRotation, Time.deltaTime * rotationSmoothTime);
    }

    void HandleCrouch()
    {
        if (crouchInput && !isCrouching && !isSwimming) // Pas d'accroupissement en natation
        {
            isCrouching = true;
            animator.SetBool("isCrouching", true);
        }
        else if (!crouchInput && isCrouching)
        {
            isCrouching = false;
            animator.SetBool("isCrouching", false);
        }
    }

    void UpdateAnimationParameters()
    {
        // Mise à jour des paramètres Horizontal et Vertical pour les Blend Trees
        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetBool("isSwimming", isSwimming);  // Activer ou désactiver le Blend Tree de natation
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = true;
            characterController.enabled = false;  // Désactiver le contrôleur de personnage pour le mouvement de natation
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = false;
            characterController.enabled = true;  // Réactiver le contrôleur de personnage quand on sort de l'eau
        }
    }
}
