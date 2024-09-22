using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    Animator animator;

    int isWalkingHash;
    int isRunningHash;

    PlayerControls playerControls;

    Vector2 currentMovement;
    bool movementPressed;
    bool runPressed;

    void Awake()
    {
        playerControls = new PlayerControls();
        
        // Détection du mouvement
        playerControls.Player.Move.performed += ctx => {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };

        // Détection du sprint
        playerControls.Player.Sprint.performed += ctx => runPressed = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.canceled += ctx => runPressed = false;
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        // Hash des animations
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleRotation()
    {
        // Rotation du personnage vers la direction du mouvement
        if (movementPressed)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
            Vector3 positionToLookAt = currentPosition + newPosition;

            // Rotation en direction du mouvement
            transform.LookAt(positionToLookAt);
        }
    }

    void HandleMovement()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);

        // Marche
        if (movementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!movementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        // Course
        if (movementPressed && runPressed && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!movementPressed || !runPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
