using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Transform playerBody;
    public Transform playerCamera;

    private enum PlayerState { Idle, Walking, Running, Crouching, Swimming, Jumping }
    private PlayerState currentState;

    public float walkSpeed = 2f;
    public float crouchSpeed = 1.5f;
    public float sprintSpeed = 5f;
    public float swimSpeed = 3f;
    public float mouseSensitivity = 100f;
    public float rotationSmoothTime = 0.1f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    private float xRotation = 0f;
    private float currentZoom = 5f;

    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool sprintInput;
    private bool crouchInput;
    private bool isSwimming = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new PlayerControls();

        playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Sprint.performed += ctx => sprintInput = true;
        playerControls.Player.Sprint.canceled += ctx => sprintInput = false;
        playerControls.Player.Crouch.performed += ctx => crouchInput = !crouchInput;

        playerControls.Player.Zoom.performed += ctx => HandleZoom(ctx.ReadValue<float>());

        // Initialisation de l'état à Idle
        currentState = PlayerState.Idle;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void Update()
    {
        HandleMouseLook();
        HandleStateTransitions();
        UpdateAnimationParameters();
        AlignBodyToCamera();
        AdjustCameraZoom();
    }

    // Méthode pour gérer les transitions d'état
    private void HandleStateTransitions()
    {
        if (isSwimming)
        {
            SetState(PlayerState.Swimming);
            HandleMovement(swimSpeed);
            return;
        }

        if (crouchInput)
        {
            SetState(PlayerState.Crouching);
            HandleMovement(crouchSpeed);
            return;
        }

        if (sprintInput && movementInput.y > 0)
        {
            SetState(PlayerState.Running);
            HandleMovement(sprintSpeed);
            return;
        }

        if (movementInput.magnitude > 0)
        {
            SetState(PlayerState.Walking);
            HandleMovement(walkSpeed);
        }
        else
        {
            SetState(PlayerState.Idle);
        }
    }

    // Méthode pour passer d'un état à un autre
    private void SetState(PlayerState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // Mettre à jour l'animation en fonction de l'état
        switch (newState)
        {
            case PlayerState.Idle:
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isSwimming", false);
                animator.SetBool("isJumping", false);
                break;
            case PlayerState.Walking:
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdle", false);
                animator.SetBool("isRunning", false);
                break;
            case PlayerState.Running:
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdle", false);
                break;
            case PlayerState.Crouching:
                animator.SetBool("isCrouching", true);
                break;
            case PlayerState.Swimming:
                animator.SetBool("isSwimming", true);
                break;
            case PlayerState.Jumping:
                animator.SetBool("isJumping", true);
                break;
        }
    }

    private void HandleMovement(float speed)
    {
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        characterController.Move(move * speed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void AlignBodyToCamera()
    {
        Vector3 cameraForward = playerCamera.forward;
        cameraForward.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetRotation, Time.deltaTime * rotationSmoothTime);
    }

    private void HandleZoom(float zoomInput)
    {
        currentZoom = Mathf.Clamp(currentZoom - zoomInput * zoomSpeed, minZoom, maxZoom);
    }

    private void AdjustCameraZoom()
    {
        playerCamera.localPosition = new Vector3(0, 0, -currentZoom);
    }

    private void UpdateAnimationParameters()
    {
        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetBool("isSwimming", isSwimming);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = false;
        }
    }
}
