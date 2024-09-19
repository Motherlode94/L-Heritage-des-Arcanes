using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    public Animator doorAnimator;  // L'Animator de la porte

    public void Interact()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        doorAnimator.SetTrigger("Open");
        isOpen = true;
        Debug.Log("La porte est ouverte");
    }

    private void CloseDoor()
    {
        doorAnimator.SetTrigger("Close");
        isOpen = false;
        Debug.Log("La porte est fermée");
    }
}
