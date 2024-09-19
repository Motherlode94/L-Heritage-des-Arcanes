using System;  // Ajoutez cette ligne pour utiliser le type Action
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public Button[] choiceButtons; // Boutons pour afficher les choix du joueur
    public QuestManager questManager;

    private NPCDialogue currentNPC;
    private int currentLine;

    public void StartDialogue(NPCDialogue npcDialogue)
    {
        currentNPC = npcDialogue;
        currentLine = 0;
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLine < currentNPC.dialogueLines.Length)
        {
            dialogueText.text = currentNPC.dialogueLines[currentLine];
            currentLine++;
        }
        else
        {
            EndDialogue();
        }
    }

    // Modifier la méthode ShowChoices pour prendre en charge les choix et les conséquences
    public void ShowChoices(string[] choices, Action[] consequences)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<Text>().text = choices[i];
                int choiceIndex = i;

                // Effacer les anciens écouteurs avant d'ajouter de nouveaux
                choiceButtons[i].onClick.RemoveAllListeners();

                // Ajouter l'action appropriée en fonction du choix sélectionné
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex, consequences[choiceIndex]));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Ajouter une méthode pour gérer la sélection des choix
    public void OnChoiceSelected(int choiceIndex, Action consequence)
    {
        Debug.Log("Choix " + choiceIndex + " sélectionné");
        consequence?.Invoke(); // Exécuter la conséquence associée au choix
        EndDialogue();
    }

    public void EndDialogue()
    {
        Debug.Log("Fin du dialogue.");
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        dialogueText.text = "";
    }
}
