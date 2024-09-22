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
    public float movementSmoothing = 0.1f;

    private Vector2 movementInput;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded, isSwimming, isJumping, isFalling;
    private int remainingJumps;
    private Rigidbody rb;
    private PlayerControls playerControls;
    private float currentSpeed;
    private bool crouchInput, sprintInput, jumpInput;
    private bool movementPressed, runPressed;
    private float currentZoom = 5f;

    public Transform cameraTransform;
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    private CapsuleCollider capsuleCollider;
    public Animator animator;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask waterMask;

    // Hash des animations
    int isWalkingForwardHash;
    int isWalkingBackwardHash;
    int isRunningHash;
    int isJumpingHash;
    int isFallingHash;
    int isLandingHash;
    int isCrouchingHash;
    int isSwimmingHash;

    private void Awake()
    {
        // Initialisation des contrôles
        playerControls = new PlayerControls();
        
        playerControls.Player.Move.performed += ctx => 
        {
            movementInput = ctx.ReadValue<Vector2>();
            movementPressed = movementInput.x != 0 || movementInput.y != 0;
        };
        
        playerControls.Player.Move.canceled += ctx => {
            movementInput = Vector2.zero;
            movementPressed = false;  // Réinitialise lorsque les touches de mouvement sont relâchées
        };

        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Crouch.performed += ctx => crouchInput = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.performed += ctx => runPressed = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.canceled += ctx => runPressed = false;

        playerControls.Player.Zoom.performed += ctx => Zoom(ctx.ReadValue<float>());

        // Hash des animations
        isWalkingForwardHash = Animator.StringToHash("isWalkingForward");
        isWalkingBackwardHash = Animator.StringToHash("isWalkingBackward");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
        isLandingHash = Animator.StringToHash("isLanding");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isSwimmingHash = Animator.StringToHash("isSwimming");
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentSpeed = walkSpeed;
        remainingJumps = maxJumps;
    }

    private void FixedUpdate()
    {
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
            jumpInput = false;
        }

        HandleJumpAndFall();
    }

    private void HandleMovement()
    {
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        UpdateSpeed();

        Vector3 direction = GetMovementDirection();
        Vector3 targetVelocity = direction * currentSpeed;

        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z), ref velocity, movementSmoothing);

        if (movementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        UpdateAnimations();
    }

    private void HandleSwimming()
    {
        Vector3 direction = GetMovementDirection();
        Vector3 targetVelocity = new Vector3(direction.x * swimSpeed, direction.y * swimSpeed, direction.z * swimSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

        animator.SetBool(isSwimmingHash, true);
        animator.SetBool(isWalkingForwardHash, false);
        animator.SetBool(isRunningHash, false);
    }

    private void UpdateAnimations()
    {
        bool isWalkingForward = movementPressed && movementInput.y > 0 && !runPressed && !crouchInput && !isJumping && !isFalling;
        bool isWalkingBackward = movementPressed && movementInput.y < 0 && !runPressed && !crouchInput && !isJumping && !isFalling;
        bool isRunning = movementPressed && runPressed && movementInput.y > 0 && !crouchInput && !isJumping && !isFalling;
        bool isIdle = !movementPressed && !isJumping && !isFalling;
        bool isCrouching = crouchInput;

        animator.SetBool(isWalkingForwardHash, isWalkingForward);
        animator.SetBool(isWalkingBackwardHash, isWalkingBackward);
        animator.SetBool(isRunningHash, isRunning);
        animator.SetBool("isIdle", isIdle);
        animator.SetBool(isCrouchingHash, isCrouching);
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * movementInput.y + right * movementInput.x;
    }

    private void UpdateSpeed()
    {
        if (crouchInput)
        {
            currentSpeed = crouchSpeed;
            HandleCrouch();
        }
        else if (runPressed && movementInput.y > 0)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    private void HandleCrouch()
    {
        capsuleCollider.height = crouchInput
            ? Mathf.Lerp(capsuleCollider.height, crouchHeight, Time.deltaTime * 10f)
            : Mathf.Lerp(capsuleCollider.height, normalHeight, Time.deltaTime * 10f);
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            remainingJumps = maxJumps;
        }
    }

    private void CheckSwimmingStatus()
    {
        isSwimming = Physics.CheckSphere(groundCheck.position, groundDistance, waterMask);
        rb.useGravity = !isSwimming;
    }

    private void HandleJump()
    {
        if (isGrounded || remainingJumps > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            remainingJumps--;
            isJumping = true;
            animator.SetBool(isJumpingHash, true);
        }
    }

    private void HandleJumpAndFall()
    {
        float verticalVelocity = rb.velocity.y;

        if (verticalVelocity < 0 && !isGrounded && !isFalling)
        {
            isJumping = false;
            isFalling = true;
            animator.SetBool(isFallingHash, true);
            animator.SetBool(isJumpingHash, false);
        }

        if (isGrounded && isFalling)
        {
            isFalling = false;
            animator.SetBool(isFallingHash, false);
            animator.SetTrigger(isLandingHash);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isLandingHash, true);
        }
    }

    private void Zoom(float scrollAmount)
    {
        currentZoom -= scrollAmount * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraTransform.localPosition.y, -currentZoom);
    }
}
