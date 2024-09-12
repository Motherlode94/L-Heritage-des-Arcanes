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

    public void ShowChoices(string[] choices)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<Text>().text = choices[i];
                int choiceIndex = i;
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log("Choice " + choiceIndex + " selected");
        // Gérer les conséquences des choix ici, par exemple affecter des quêtes
        if (choiceIndex == 0)
        {
            questManager.CompleteQuest("Find the Magic Stone");
        }
        EndDialogue();
    }

    public void EndDialogue()
    {
        Debug.Log("End of dialogue.");
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        dialogueText.text = "";
    }
}
