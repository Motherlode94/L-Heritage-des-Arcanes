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
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isLanding = false;

    public float walkSpeed = 2f;
    public float crouchSpeed = 1.5f;
    public float sprintSpeed = 5f;
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
        HandleAnimations();
        HandleCrouch();
        AlignBodyToCamera();
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        if (sprintInput && movementInput.y > 0 && !isCrouching)
        {
            isSprinting = true;
            characterController.Move(move * sprintSpeed * Time.deltaTime);
        }
        else if (isCrouching)
        {
            if (movementInput.magnitude > 0)
            {
                animator.SetBool("CrouchMoving", true);  // Utiliser le blend tree pour les mouvements accroupis
            }
            characterController.Move(move * crouchSpeed * Time.deltaTime);
        }
        else
        {
            isSprinting = false;
            characterController.Move(move * walkSpeed * Time.deltaTime);
        }

        isWalkingFW = (movementInput.y > 0);
        isWalkingBW = (movementInput.y < 0);

        HandleJumpAndFall();
    }

    void HandleJumpAndFall()
    {
        // Gestion du saut, de la chute, et de l'atterrissage
        if (jumpInput && characterController.isGrounded && !isJumping)
        {
            isJumping = true;
            isFalling = false;
            isLanding = false;

            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
            animator.ResetTrigger("isLanding");

            jumpInput = false;
        }
        else if (!characterController.isGrounded && !isJumping)
        {
            isFalling = true;
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
        else if (characterController.isGrounded && isFalling)
        {
            isFalling = false;
            isLanding = true;

            animator.SetTrigger("isLanding");  // Déclencher l'animation d'atterrissage
            animator.SetBool("isFalling", false);
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

    void AlignBodyToCamera()
    {
        // On ne tourne que le corps, pas la caméra
        Vector3 cameraForward = playerCamera.forward;
        cameraForward.y = 0;  // Éviter que le joueur se penche ou se lève

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetRotation, Time.deltaTime * rotationSmoothTime);
    }

    void HandleAnimations()
    {
        // Gestion des animations accroupies via blend tree
        if (isCrouching)
        {
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);

            if (movementInput == Vector2.zero)
            {
                animator.Play("CrouchIdle");
            }
            else
            {
                animator.SetBool("CrouchMoving", true);  // Le BlendTree gère les mouvements
            }
        }
        else
        {
            // Gestion des animations classiques
            isIdle = movementInput == Vector2.zero && !isSprinting && !isJumping && !isFalling && !isLanding;
            animator.SetBool("Idle", isIdle);
            animator.SetBool("WalkFW", isWalkingFW);
            animator.SetBool("WalkBW", isWalkingBW);
            animator.SetBool("Sprint", isSprinting);
        }
    }

    void HandleCrouch()
    {
        if (crouchInput && !isCrouching)
        {
            isCrouching = true;
            animator.Play("CrouchStart");  // Animation de début d'accroupissement
            Invoke(nameof(StartCrouchIdle), animator.GetCurrentAnimatorStateInfo(0).length);  // Passer à l'Idle accroupi après l'animation de début
        }
        else if (!crouchInput && isCrouching)
        {
            isCrouching = false;
            animator.Play("CrouchEnd");  // Animation de fin d'accroupissement
        }
    }

    void StartCrouchIdle()
    {
        if (isCrouching)
        {
            animator.Play("CrouchIdle");  // Jouer l'animation Idle après le Start
        }
    }
}
