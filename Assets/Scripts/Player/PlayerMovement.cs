using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 10f;
    public float jumpForce = 10f;
    public float movementSmoothing = 0.1f;

    private Vector2 movementInput;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;
    private Rigidbody rb;
    private PlayerControls playerControls;
    private float currentSpeed;
    private bool crouchInput, sprintInput, jumpInput;

    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    private CapsuleCollider capsuleCollider;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero; // Ajout pour arrêter le mouvement
        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Crouch.performed += ctx => crouchInput = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.performed += ctx => sprintInput = ctx.ReadValueAsButton();
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
    }

    private void FixedUpdate()
    {
        HandleMovement();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Apply air resistance when not grounded
        if (!isGrounded)
        {
            rb.velocity *= 0.9f;
        }

        // Handle jumping if grounded
        if (isGrounded && jumpInput)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Réinitialise la vitesse verticale avant de sauter
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInput = false;
        }
    }

    private void HandleMovement()
    {
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        // Switch between sprint, crouch, and walk
        if (sprintInput)
        {
            currentSpeed = sprintSpeed;
        }
        else if (crouchInput)
        {
            currentSpeed = crouchSpeed;
            HandleCrouch();
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Calculate movement direction
        Vector3 moveDirection = transform.forward * movementInput.y * currentSpeed + transform.right * movementInput.x * currentSpeed;
        Vector3 targetVelocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    private void HandleCrouch()
    {
        if (crouchInput)
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, crouchHeight, Time.deltaTime * crouchSpeed);
        }
        else
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, normalHeight, Time.deltaTime * crouchSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
