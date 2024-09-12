using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public Transform firstPersonTarget;  // Position et rotation cible pour la caméra First Person
    public Transform thirdPersonTarget;  // Position et rotation cible pour la caméra Third Person
    public Transform topDownTarget;      // Position et rotation cible pour la vue d'aigle

    public Camera mainCamera;            // La caméra principale qui va effectuer les transitions

    public float transitionDuration = 1.5f; // Durée de la transition en secondes

    private bool isTransitioning = false;
    private Transform currentTarget;
    private float transitionProgress;

    void Start()
    {
        // Par défaut, activer la vue à la troisième personne
        SetThirdPersonView();
    }

    public void OnSwitchCameraView(InputAction.CallbackContext context)
    {
        if (context.performed && !isTransitioning)
        {
            if (currentTarget == firstPersonTarget)
            {
                SetThirdPersonView();
            }
            else if (currentTarget == thirdPersonTarget)
            {
                SetTopDownView();
            }
            else if (currentTarget == topDownTarget)
            {
                SetFirstPersonView();
            }
        }
    }

    private void SetFirstPersonView()
    {
        StartTransition(firstPersonTarget);
    }

    private void SetThirdPersonView()
    {
        StartTransition(thirdPersonTarget);
    }

    private void SetTopDownView()
    {
        StartTransition(topDownTarget);
    }

    private void StartTransition(Transform newTarget)
    {
        currentTarget = newTarget;
        transitionProgress = 0f;
        isTransitioning = true;
    }

    void Update()
    {
        if (isTransitioning)
        {
            PerformTransition();
        }
    }

    private void PerformTransition()
    {
        transitionProgress += Time.deltaTime / transitionDuration;

        // Interpolation de la position et de la rotation de la caméra
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, currentTarget.position, transitionProgress);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, currentTarget.rotation, transitionProgress);

        // Lorsque la transition est terminée
        if (transitionProgress >= 1f)
        {
            isTransitioning = false;
        }
    }
}
