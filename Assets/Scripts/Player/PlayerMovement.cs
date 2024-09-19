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

    public Transform cameraTransform; // Référence à la caméra pour les déplacements relatifs à la caméra
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
        playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero; // Arrête le mouvement quand aucune entrée
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
        rb.freezeRotation = true;  // On empêche la rotation causée par la physique
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentSpeed = walkSpeed;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        CheckGroundStatus();

        if (isGrounded && jumpInput)
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        UpdateSpeed();

        // Calcul du déplacement relatif à la caméra
        Vector3 direction = GetMovementDirection();
        Vector3 targetVelocity = new Vector3(direction.x * currentSpeed, rb.velocity.y, direction.z * currentSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    private Vector3 GetMovementDirection()
    {
        // Obtenir la direction de déplacement relative à la caméra
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Appliquer les entrées aux directions
        Vector3 moveDir = forward * movementInput.y + right * movementInput.x;
        return moveDir;
    }

    private void UpdateSpeed()
    {
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
    }

    private void HandleCrouch()
    {
        if (crouchInput)
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, crouchHeight, Time.deltaTime * 10f);
        }
        else
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, normalHeight, Time.deltaTime * 10f);
        }
    }

    private void CheckGroundStatus()
    {
        // Vérification si le joueur est au sol
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Réinitialisation de la vitesse verticale avant de sauter
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpInput = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
