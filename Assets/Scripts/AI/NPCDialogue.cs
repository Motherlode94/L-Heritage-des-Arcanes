using System;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogueLines;
    public string[] playerChoices;
    public Action[] choiceConsequences; // Actions à déclencher pour chaque choix
    public DialogueManager dialogueManager;

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(this);
        if (playerChoices.Length > 0)
        {
            dialogueManager.ShowChoices(playerChoices, choiceConsequences);
        }
    }
}
