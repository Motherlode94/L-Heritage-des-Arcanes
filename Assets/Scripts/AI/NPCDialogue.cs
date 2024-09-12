using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogueLines;
    public string[] playerChoices; // Les choix que le joueur peut faire
    public DialogueManager dialogueManager;

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(this);
        if (playerChoices.Length > 0)
        {
            dialogueManager.ShowChoices(playerChoices);
        }
    }
}
