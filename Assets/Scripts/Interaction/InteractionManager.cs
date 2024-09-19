using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float interactionRange = 3f;  // Distance à laquelle le joueur peut interagir
    public LayerMask interactionLayer;  // Les couches avec lesquelles on peut interagir
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Interact.performed += ctx => Interact();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        RaycastHit hit;
        // Lance un rayon à partir de la caméra pour détecter les objets interactifs
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Affiche une invite pour interagir (Ex: Appuyez sur E pour interagir)
                Debug.Log("Appuyez sur E pour interagir avec " + hit.collider.gameObject.name);
            }
        }
    }

    private void Interact()
    {
        RaycastHit hit;
        // Même logique que pour la détection
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Appelle la méthode d'interaction de l'objet
                interactable.Interact();
            }
        }
    }
}
