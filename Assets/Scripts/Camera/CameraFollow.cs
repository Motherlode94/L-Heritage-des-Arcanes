using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;           // Référence au transform du joueur
    public Vector3 offset = new Vector3(0, 3, -5); // Décalage de la caméra par rapport au joueur
    public float smoothSpeed = 0.125f; // Vitesse de lissage du mouvement de la caméra
    public bool followRotation = true; // Si la caméra doit suivre la rotation du joueur

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calculer la position cible de la caméra
            Vector3 desiredPosition = player.position + offset;
            
            // Lissage de la position de la caméra pour des mouvements plus fluides
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            
            // Appliquer la nouvelle position à la caméra
            transform.position = smoothedPosition;

            // Optionnel : suivre la rotation du joueur si activé
            if (followRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation, smoothSpeed);
            }

            // Regarder le joueur
            // Optionnel : activer si la caméra doit toujours pointer vers le joueur
            // transform.LookAt(player);
        }
    }
}
