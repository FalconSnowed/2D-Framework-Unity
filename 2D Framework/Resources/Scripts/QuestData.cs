using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public string questType;
    public string questId;
    public string questTitle;
    public string questDescription;
    [TextArea(2, 5)] public string descriptions;
    public int requiredKills = 4;
    [HideInInspector] public int currentKills = 0;

    public bool isMainQuest;
    public bool isCompleted;
}
