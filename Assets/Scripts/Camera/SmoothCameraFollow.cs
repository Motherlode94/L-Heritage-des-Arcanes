using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;            // Le joueur ou l'objet que la caméra suit
    public Vector3 offset = new Vector3(0, 3, -6);  // Décalage de la caméra par rapport au joueur
    public float smoothSpeed = 0.125f;  // Vitesse de lissage
    public float rotationSpeed = 100f;  // Vitesse de rotation de la caméra autour du joueur
    public float minZoom = 2f;          // Distance minimum pour zoom
    public float maxZoom = 8f;          // Distance maximum pour zoom
    public float zoomSpeed = 4f;        // Vitesse de zoom

    private float currentZoom = 5f;     // Distance actuelle de zoom
    private float currentYaw = 0f;      // Rotation actuelle de la caméra autour de l'axe Y
    private float currentPitch = 0f;    // Inclinaison de la caméra (haut/bas)

    void Update()
    {
        // Gérer le zoom avec la molette de la souris
        float zoomChange = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= zoomChange * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Gérer la rotation de la caméra autour du joueur avec la souris
        if (Input.GetMouseButton(1)) // Click droit pour faire tourner la caméra
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentYaw += mouseX;

            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentPitch -= mouseY;
            currentPitch = Mathf.Clamp(currentPitch, -30f, 60f);  // Limiter l'inclinaison
        }
    }

    void LateUpdate()
    {
        // Calculer la position désirée avec le zoom
        Vector3 desiredPosition = target.position - (Quaternion.Euler(currentPitch, currentYaw, 0) * Vector3.forward * currentZoom) + offset;

        // Lissage de la position de la caméra
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Faire en sorte que la caméra regarde toujours vers le joueur
        transform.LookAt(target.position + Vector3.up * 1.5f);  // Ajuste l'axe pour que la caméra regarde légèrement au-dessus du joueur
    }
}
