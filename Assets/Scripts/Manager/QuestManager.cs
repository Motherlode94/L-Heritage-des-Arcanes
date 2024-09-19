using UnityEngine;
using System.Collections.Generic;

public class Quest
{
    public string questName;
    public string description; // Ajouter une description de la quête
    public bool isComplete = false;
    public int rewardPoints; // Récompenses pour la quête

    public Quest(string name, string desc, int reward)
    {
        questName = name;
        description = desc;
        rewardPoints = reward;
    }
}

public class QuestManager : MonoBehaviour
{
    public List<Quest> questList = new List<Quest>();
    public ScoreManager scoreManager; // Référence pour ajouter des récompenses

    public void AddQuest(string questName, string description, int rewardPoints)
    {
        Quest newQuest = new Quest(questName, description, rewardPoints);
        questList.Add(newQuest);
        Debug.Log("New Quest Added: " + questName);
    }

    public void CompleteQuest(string questName)
    {
        foreach (Quest quest in questList)
        {
            if (quest.questName == questName && !quest.isComplete)
            {
                quest.isComplete = true;
                Debug.Log("Quest Completed: " + questName);
                RewardPlayer(quest.rewardPoints); // Donne la récompense
            }
        }
    }

    private void RewardPlayer(int rewardPoints)
    {
        if (scoreManager != null)
        {
            scoreManager.AddScore(rewardPoints); // Ajoute des points de score en récompense
            Debug.Log("Player rewarded with: " + rewardPoints + " points.");
        }
    }
}
