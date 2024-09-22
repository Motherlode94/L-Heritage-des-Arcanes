using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera thirdPersonCamera;
    public Camera topDownCamera;
    public float transitionDuration = 1.5f;
    public AnimationCurve transitionCurve;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.SwitchCameraView.performed += OnSwitchCameraView;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        SetThirdPersonView();  // Start with third-person view
    }

    private void OnSwitchCameraView(InputAction.CallbackContext context)
    {
        if (thirdPersonCamera.enabled)
        {
            StartCoroutine(SmoothTransition(thirdPersonCamera, topDownCamera, transitionDuration));
        }
        else if (topDownCamera.enabled)
        {
            StartCoroutine(SmoothTransition(topDownCamera, thirdPersonCamera, transitionDuration));
        }
    }

    private IEnumerator SmoothTransition(Camera fromCamera, Camera toCamera, float duration)
    {
        float elapsedTime = 0f;

        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;
        float startFOV = fromCamera.fieldOfView;

        Vector3 endPosition = toCamera.transform.position;
        Quaternion endRotation = toCamera.transform.rotation;
        float endFOV = toCamera.fieldOfView;

        toCamera.enabled = true;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / duration);

            toCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            toCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            toCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);

            yield return null;
        }

        fromCamera.enabled = false;
    }

    private void SetThirdPersonView()
    {
        thirdPersonCamera.enabled = true;
        topDownCamera.enabled = false;
    }

    private void SetTopDownView()
    {
        thirdPersonCamera.enabled = false;
        topDownCamera.enabled = true;
    }
}
