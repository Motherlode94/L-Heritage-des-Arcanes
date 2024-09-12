using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform newCameraPosition;  // La nouvelle position/rotation de la caméra après avoir franchi la zone
    public float transitionDuration = 1.5f;  // Durée de la transition de la caméra

    private ThirdPersonCamera cameraController;  // Référence au script de la caméra

    void Start()
    {
        cameraController = Camera.main.GetComponent<ThirdPersonCamera>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Appeler la méthode pour changer la position de la caméra
            cameraController.StartCameraTransition(newCameraPosition, transitionDuration);
        }
    }
}
