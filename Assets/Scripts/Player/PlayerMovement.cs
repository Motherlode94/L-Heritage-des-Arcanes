using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 10f;
    public float swimSpeed = 3f;
    public float turnSpeed = 200f;
    public float jumpForce = 10f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    public int maxJumps = 2;
    public float stamina = 100f;
    public float staminaRegenRate = 5f;
    public float jumpHoldTime = 0.2f;
    public float sprintStaminaCost = 10f;
    public float dodgeStaminaCost = 15f;
    public float attackStaminaCost = 20f;
    public float movementSmoothing = 0.1f;
    public float gravityMultiplier = 2f; // For faster falling

    private Vector2 movementInput;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded, isSwimming, isJumping, isFalling, isCrouching, isAttacking, isDodging;
    private int remainingJumps;
    private Rigidbody rb;
    private PlayerControls playerControls;
    private float currentSpeed;
    private bool crouchInput, sprintInput, jumpInput;
    private bool movementPressed, runPressed, attackInput, dodgeInput;
    private float currentZoom = 5f;
    private float jumpTimer;

    public Transform cameraTransform;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask waterMask;

    public Animator animator;
    private CapsuleCollider capsuleCollider;

    public float normalHeight = 2f;
    public float crouchHeight = 1f;

    // Animation hashes
    private int isWalkingHash, isRunningHash, isJumpingHash, isFallingHash, isLandingHash, isCrouchingHash, isSwimmingHash, isAttackingHash;

    private void Awake()
    {
        // Initialize input controls
        playerControls = new PlayerControls();

        playerControls.Player.Move.performed += ctx =>
        {
            movementInput = ctx.ReadValue<Vector2>();
            movementPressed = movementInput.x != 0 || movementInput.y != 0;
        };

        playerControls.Player.Move.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            movementPressed = false;
        };

        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Crouch.performed += ctx => crouchInput = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.performed += ctx => runPressed = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.canceled += ctx => runPressed = false;

        playerControls.Player.Attack.performed += ctx => attackInput = true;
        playerControls.Player.Dodge.performed += ctx => dodgeInput = true;

        playerControls.Player.Zoom.performed += ctx => Zoom(ctx.ReadValue<float>());
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentSpeed = walkSpeed;
        remainingJumps = maxJumps;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smoother physics
    }

    private void Update()
    {
        // Cache input in Update() for better performance
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();
        jumpInput = playerControls.Player.Jump.triggered;
        crouchInput = playerControls.Player.Crouch.IsPressed();
        sprintInput = playerControls.Player.Sprint.IsPressed();
        attackInput = playerControls.Player.Attack.triggered;
        dodgeInput = playerControls.Player.Dodge.triggered;
    }

    private void FixedUpdate()
    {
        HandleStamina();
        CheckGroundStatus();
        CheckSwimmingStatus();

        if (isSwimming)
        {
            HandleSwimming();
        }
        else
        {
            HandleMovement();
        }

        if (jumpInput && !isSwimming)
        {
            HandleJump();
        }

        if (attackInput)
        {
            HandleAttack();
        }

        if (dodgeInput)
        {
            HandleDodge();
        }

        HandleJumpAndFall();
        UpdateAnimations();
    }

    private void CheckSwimmingStatus()
    {
        // Check if player is in water
        isSwimming = Physics.CheckSphere(groundCheck.position, groundDistance, waterMask);
        rb.useGravity = !isSwimming;
        
        // Update animation accordingly
        animator.SetBool("isSwimming", isSwimming);

        // Adjust movement speed while swimming
        if (isSwimming)
        {
            currentSpeed = swimSpeed;
        }
    }

    private void HandleMovement()
    {
        // Ensure vertical velocity is reset when grounded
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        UpdateSpeed();
        Vector3 direction = GetMovementDirection();

        // Smooth movement and rotation
        Vector3 targetVelocity = direction * currentSpeed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z), ref velocity, movementSmoothing);

        if (movementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void HandleJump()
    {
        if (remainingJumps > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            remainingJumps--;
            isJumping = true;
            jumpTimer = jumpHoldTime;
        }
        jumpInput = false;
    }

    private void HandleAttack()
    {
        if (stamina >= attackStaminaCost && !isAttacking)
        {
            isAttacking = true;
            stamina -= attackStaminaCost;
            animator.SetTrigger("Attack"); // Assuming you have an "Attack" trigger in the Animator
        }
        attackInput = false;
    }

    private void HandleDodge()
    {
        if (stamina >= dodgeStaminaCost && !isDodging)
        {
            isDodging = true;
            stamina -= dodgeStaminaCost;
            animator.SetTrigger("Dodge");  // Assuming you have a "Dodge" trigger in the Animator
        }
        dodgeInput = false;
    }

    private void HandleStamina()
    {
        if (runPressed && stamina > 0)
        {
            stamina -= sprintStaminaCost * Time.deltaTime;
        }
        else if (!isDodging && !isAttacking)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 100f);
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            remainingJumps = maxJumps;
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    private void HandleJumpAndFall()
    {
        // Get the vertical velocity of the player (y-axis)
        float verticalVelocity = rb.velocity.y;

        // Check if the player is falling (negative vertical velocity and not grounded)
        if (verticalVelocity < 0 && !isGrounded && !isFalling)
        {
            // Player starts falling
            isJumping = false;
            isFalling = true;
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }

        // Check if the player has landed
        if (isGrounded && isFalling)
        {
            // Player has landed
            isFalling = false;
            animator.SetBool("isFalling", false);
            animator.SetTrigger("isLanding");  // Use a trigger for landing animation
        }
    }

    private void UpdateAnimations()
    {
        bool isWalkingForward = movementInput.y > 0 && !runPressed && !crouchInput && !isJumping && !isFalling;
        bool isWalkingBackward = movementInput.y < 0 && !runPressed && !crouchInput && !isJumping && !isFalling;
        bool isRunning = movementInput.y > 0 && runPressed && !crouchInput && !isJumping && !isFalling;
        bool isIdle = movementInput == Vector2.zero && !isJumping && !isFalling;

        animator.SetBool("isWalkingForward", isWalkingForward);
        animator.SetBool("isWalkingBackward", isWalkingBackward);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isIdle", isIdle);

        animator.SetBool("isCrouching", crouchInput);

        if (isSwimming)
        {
            animator.SetBool("isSwimming", true);
        }
        else
        {
            animator.SetBool("isSwimming", false);
        }
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;

        return forward * movementInput.y + right * movementInput.x;
    }

    private void UpdateSpeed()
    {
        if (crouchInput)
        {
            currentSpeed = crouchSpeed;
        }
        else if (runPressed && stamina > 0)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    private void HandleSwimming()
    {
        Vector3 direction = GetMovementDirection();
        Vector3 targetVelocity = new Vector3(direction.x * swimSpeed, direction.y * swimSpeed, direction.z * swimSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        
        animator.SetBool("isSwimming", true);
    }

    private bool CanStandUp()
    {
        return !Physics.CheckCapsule(transform.position, transform.position + Vector3.up * normalHeight, capsuleCollider.radius, groundMask);
    }

    private void Zoom(float scrollAmount)
    {
        Camera.main.fieldOfView -= scrollAmount * zoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
    }
}
