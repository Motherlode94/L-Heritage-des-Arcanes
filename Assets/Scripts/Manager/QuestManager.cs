using UnityEngine;
using System.Collections.Generic;

public class Quest
{
    public string questName;
    public bool isComplete = false;

    public Quest(string name)
    {
        questName = name;
    }
}

public class QuestManager : MonoBehaviour
{
    public List<Quest> questList = new List<Quest>();

    public void AddQuest(string questName)
    {
        Quest newQuest = new Quest(questName);
        questList.Add(newQuest);
        Debug.Log("New Quest: " + questName);
    }

    public void CompleteQuest(string questName)
    {
        foreach (Quest quest in questList)
        {
            if (quest.questName == questName)
            {
                quest.isComplete = true;
                Debug.Log("Quest Completed: " + questName);
            }
        }
    }
}
