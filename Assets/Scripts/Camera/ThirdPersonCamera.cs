using System.Collections;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;            // Référence au joueur que la caméra suit
    public Vector3 offset;              // Décalage par rapport au joueur
    public float smoothSpeed = 0.125f;  // Vitesse de lissage du mouvement de la caméra
    public float rotationSpeed = 5f;    // Vitesse de rotation de la caméra

    private bool isTransitioning = false;  // Indique si la caméra est en train de transitionner
    private Transform targetPosition;      // Position cible de la caméra pendant la transition
    private float transitionDuration;      // Durée de la transition
    private float transitionProgress = 0f; // Avancement de la transition

    void LateUpdate()
    {
        if (!isTransitioning)
        {
            // Contrôle normal de la caméra (suivre le joueur avec un décalage)
            FollowPlayer();
        }
        else
        {
            // Transition lissée vers une nouvelle position
            PerformTransition();
        }
    }

    void FollowPlayer()
    {
        // Mouvement normal de la caméra (suivant le joueur)
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Faire regarder la caméra vers le joueur
        transform.LookAt(player.position);
    }

    public void StartCameraTransition(Transform newTargetPosition, float duration)
    {
        targetPosition = newTargetPosition;
        transitionDuration = duration;
        transitionProgress = 0f;
        isTransitioning = true;
    }

    void PerformTransition()
    {
        transitionProgress += Time.deltaTime / transitionDuration;

        // Transition lissée vers la nouvelle position et rotation
        transform.position = Vector3.Lerp(transform.position, targetPosition.position, transitionProgress);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition.rotation, transitionProgress);

        // Lorsque la transition est terminée
        if (transitionProgress >= 1f)
        {
            isTransitioning = false;
        }
    }
}
