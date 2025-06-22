using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public List<Quest> activeQuests;

    public void CompleteQuest(Quest quest)
    {
        quest.isCompleted = true;
        UnityEngine.Debug.Log("Quest Completed: " + quest.questName);
    }
}

[System.Serializable]
public class Quest
{
    public string questName;
    public string description;
    public bool isCompleted;
}
