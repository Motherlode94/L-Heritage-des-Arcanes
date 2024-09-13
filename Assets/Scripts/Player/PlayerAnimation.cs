using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Transform playerBody;
    public Transform playerCamera;

    private bool isIdle = true;
    private bool isWalkingFW = false;
    private bool isWalkingBW = false;
    private bool isJumping = false;
    private bool isSprinting = false;

    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float rotationSmoothTime = 0.1f;
    private float xRotation = 0f;

    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 movementInput = Vector2.zero; // Initialiser correctement
    private Vector2 lookInput;
    private bool jumpInput;
    private bool sprintInput;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new PlayerControls();

        // Actions liées au mouvement
        playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero; // Réinitialisation du mouvement

        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Sprint.performed += ctx => sprintInput = true;
        playerControls.Player.Sprint.canceled += ctx => sprintInput = false;
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
        HandleAnimations();
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        if (sprintInput && movementInput.y > 0)
        {
            isSprinting = true;
            characterController.Move(move * sprintSpeed * Time.deltaTime);
        }
        else
        {
            isSprinting = false;
            characterController.Move(move * walkSpeed * Time.deltaTime);
        }

        isWalkingFW = (movementInput.y > 0);
        isWalkingBW = (movementInput.y < 0);

        if (jumpInput)
        {
            isJumping = true;
            jumpInput = false; // Réinitialiser le saut
        }
        else
        {
            isJumping = false;
        }
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

    void HandleAnimations()
    {
        // Détecter si le joueur est inactif
        isIdle = movementInput == Vector2.zero && !isSprinting && !isJumping;

        animator.SetBool("Idle", isIdle);
        animator.SetBool("WalkFW", isWalkingFW);
        animator.SetBool("WalkBW", isWalkingBW);
        animator.SetBool("Jump", isJumping);
        animator.SetBool("Sprint", isSprinting);
    }
}
