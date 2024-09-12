using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator; // Référence à l'Animator du personnage
    public Transform playerBody; // Référence au corps du joueur pour la rotation
    public Transform playerCamera; // Référence à la caméra pour contrôler la vue

    // Paramètres d'animation
    private bool isIdle = true;
    private bool isWalkingFW = false;
    private bool isWalkingBW = false;
    private bool isJumping = false;
    private bool isSprinting = false;

    // Variables pour le mouvement et la souris
    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float rotationSmoothTime = 0.1f; // Pour la rotation fluide
    private float xRotation = 0f;

    private CharacterController characterController;
    private PlayerControls playerControls; // Classe générée pour les actions d'input
    private Vector2 movementInput; // Stocker l'input de mouvement
    private Vector2 lookInput; // Stocker l'input de la souris
    private bool jumpInput;
    private bool sprintInput;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new PlayerControls(); // Initialiser le système d'Input

        // Abonner les actions de mouvement
        playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Jump.performed += ctx => jumpInput = true;
        playerControls.Player.Sprint.performed += ctx => sprintInput = true;
        playerControls.Player.Sprint.canceled += ctx => sprintInput = false; // Annuler le sprint quand la touche est relâchée
    }

    void OnEnable()
    {
        playerControls.Player.Enable(); // Activer les inputs
    }

    void OnDisable()
    {
        playerControls.Player.Disable(); // Désactiver les inputs quand l'objet est désactivé
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleAnimations();
    }

    // Gérer les mouvements du joueur
    void HandleMovement()
    {
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        if (sprintInput && movementInput.y > 0)
        {
            // Sprinter vers l'avant
            isSprinting = true;
            characterController.Move(move * sprintSpeed * Time.deltaTime);
        }
        else
        {
            // Marcher
            isSprinting = false;
            characterController.Move(move * walkSpeed * Time.deltaTime);
        }

        // Détecter si le joueur marche en avant ou en arrière
        isWalkingFW = (movementInput.y > 0);
        isWalkingBW = (movementInput.y < 0);

        // Gérer le saut
        if (jumpInput)
        {
            isJumping = true;
            jumpInput = false; // Réinitialiser le saut après avoir déclenché l'animation
        }
        else
        {
            isJumping = false;
        }
    }

    // Gérer la rotation avec la souris
    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limiter la rotation verticale (la vue de la caméra)

        // Appliquer la rotation à la caméra (vue du joueur)
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotation fluide du corps du joueur
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // Gérer les animations en fonction des actions du joueur
    void HandleAnimations()
    {
        // Mettre à jour les booléens d'animation
        animator.SetBool("Idle", isIdle);
        animator.SetBool("WalkFW", isWalkingFW);
        animator.SetBool("WalkBW", isWalkingBW);
        animator.SetBool("Jump", isJumping);
        animator.SetBool("Sprint", isSprinting);

        // Si le joueur ne bouge pas
        isIdle = !isWalkingFW && !isWalkingBW && !isSprinting;
    }
}
