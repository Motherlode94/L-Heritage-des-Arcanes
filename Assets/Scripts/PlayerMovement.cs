using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movementInput;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Cette méthode est appelée lorsque l'input "Move" est déclenché
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Mouvement en fonction de l'input récupéré, conversion du mouvement 2D en 3D (x, z)
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
}
