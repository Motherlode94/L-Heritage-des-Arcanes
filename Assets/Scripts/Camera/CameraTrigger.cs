using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform newCameraPosition;  // Nouvelle position cible pour la caméra
    public float transitionDuration = 1.5f;  // Durée de la transition

    private ThirdPersonCamera cameraController;

    void Start()
    {
        // Trouver le script ThirdPersonCamera attaché à la caméra principale
        cameraController = Camera.main?.GetComponent<ThirdPersonCamera>();
        if (cameraController == null)
        {
            Debug.LogError("ThirdPersonCamera script not found on the Main Camera!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si le joueur entre dans le trigger et que le script de caméra est présent
        if (other.CompareTag("Player") && cameraController != null)
        {
            // Déclencher la transition de la caméra vers la nouvelle position
            cameraController.StartCameraTransition(newCameraPosition, transitionDuration);
        }
    }
}
