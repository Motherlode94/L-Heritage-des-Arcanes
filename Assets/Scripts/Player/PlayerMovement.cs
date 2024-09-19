using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 10f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public int maxJumps = 2;  // Permet le double saut
    public float movementSmoothing = 0.1f;

    private Vector2 movementInput;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;
    private int remainingJumps;
    private Rigidbody rb;
    private PlayerControls playerControls;
    private float currentSpeed;
    private bool crouchInput, sprintInput, jumpInput;

    public Transform cameraTransform; // Référence à la caméra pour les déplacements relatifs à la caméra
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    private CapsuleCollider capsuleCollider;
    public Animator animator;  // Référence à l'Animator

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
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
        rb.freezeRotation = true;  // Empêche la rotation causée par la physique
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentSpeed = walkSpeed;
        remainingJumps = maxJumps;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        CheckGroundStatus();

        if (jumpInput)
        {
            HandleJump();
            jumpInput = false;  // Réinitialiser après le saut
        }
    }

    private void HandleMovement()
    {
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        UpdateSpeed();

        Vector3 direction = GetMovementDirection();
        Vector3 targetVelocity = new Vector3(direction.x * currentSpeed, rb.velocity.y, direction.z * currentSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

        // Gestion des animations de shuffle (gauche/droite)
        if (animator != null)
        {
            if (movementInput.x < 0)  // Déplacement à gauche
            {
                animator.SetBool("isShufflingLeft", true);
                animator.SetBool("isShufflingRight", false);
            }
            else if (movementInput.x > 0)  // Déplacement à droite
            {
                animator.SetBool("isShufflingLeft", false);
                animator.SetBool("isShufflingRight", true);
            }
            else  // Pas de shuffle
            {
                animator.SetBool("isShufflingLeft", false);
                animator.SetBool("isShufflingRight", false);
            }
        }
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
        else if (sprintInput && !crouchInput)  // Sprint désactivé si accroupi
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
        if (crouchInput)
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, crouchHeight, Time.deltaTime * 10f);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, new Vector3(0, crouchHeight, 0), Time.deltaTime * 10f); // Ajustement caméra
        }
        else
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, normalHeight, Time.deltaTime * 10f);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, new Vector3(0, normalHeight, 0), Time.deltaTime * 10f); // Réinitialisation de la caméra
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            remainingJumps = maxJumps;  // Réinitialiser les sauts lorsqu'au sol
        }
    }

    private void HandleJump()
    {
        if (isGrounded || remainingJumps > 0)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // Réinitialisation verticale avant le saut
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (remainingJumps > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // Réinitialisation pour double saut
                rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            }

            remainingJumps--;
            jumpInput = false;
        }
    }

    public void InteractWithObject(string interactionType)
    {
        if (animator != null)
        {
            switch (interactionType)
            {
                case "Loot":
                    animator.SetTrigger("Loot");
                    break;
                case "OpeningStart":
                    animator.SetTrigger("OpeningStart");
                    break;
                case "OpeningEnd":
                    animator.SetTrigger("OpeningEnd");
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
