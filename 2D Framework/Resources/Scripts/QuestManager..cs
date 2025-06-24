using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<QuestData> activeQuests = new List<QuestData>();
    private HashSet<string> collectedArtifacts = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddQuest(QuestData newQuest)
    {
        if (!activeQuests.Contains(newQuest))
        {
            activeQuests.Add(newQuest);
            Debug.Log("Nouvelle quête ajoutée : " + newQuest.questTitle);
        }
    }

    [System.Obsolete]
    public void CollectArtifact(string artifactId)
    {
        if (!collectedArtifacts.Contains(artifactId))
        {
            collectedArtifacts.Add(artifactId);
            Debug.Log($"✅ Artéfact {artifactId} collecté ({collectedArtifacts.Count}/3)");

            if (collectedArtifacts.Count >= 3)
            {
                UnlockNextArea();
            }
        }
    }

    [System.Obsolete]
    private void UnlockNextArea()
    {
        Debug.Log("✨ Tous les artéfacts ont été collectés ! Une nouvelle zone est accessible.");

        // Active un portail, un trigger, ou une falaise qui s’effondre
        GameObject gate = GameObject.Find("ZoneGate");
        if (gate != null)
        {
            gate.SetActive(false); // désactive un mur ou barrière
        }

        // Bonus : feedback
        if (FindObjectOfType<QuestRewardPopup>() is QuestRewardPopup popup)
        {
            popup.Show("🚪 Une nouvelle zone s'ouvre !");
        }
    }

    public void RegisterKill(string questId)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questId == questId && !quest.isCompleted)
            {
                quest.currentKills++;
                Debug.Log($"Progression de la quête {quest.questTitle} : {quest.currentKills}/{quest.requiredKills}");

                if (quest.currentKills >= quest.requiredKills)
                {
                    CompleteQuest(quest);
                }
                if (rewardPopup != null)
                {
                    rewardPopup.Show($"⚔️ {quest.questTitle} : {quest.currentKills}/{quest.requiredKills}", 2f);
                }

            }
        }
    }
    private QuestRewardPopup rewardPopup;

    [System.Obsolete]
    private void Start()
    {
        rewardPopup = FindObjectOfType<QuestRewardPopup>();
    }

    public void CompleteQuest(QuestData quest)
    {
        if (activeQuests.Contains(quest) && !quest.isCompleted)
        {
            quest.isCompleted = true;

            // Ajout de récompense : XP ou item
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (player != null)
            {
                if (player.TryGetComponent(out LevelSystem level))
                {
                    level.AddExperience(100);
                    // ✅ Gagne 100 XP
                }

                // Ajout d’une potion dans l’inventaire si tu veux
                // var potion = Resources.Load<ItemData>("Potion"); // ou un autre moyen d’accès
                // InventoryManager.Instance.AddItem(potion);
            }

            Debug.Log("Quête complétée : " + quest.questTitle);
            if (rewardPopup != null)
            {
                rewardPopup.Show($"🎉 Quête terminée : {quest.questTitle} +100 XP !");
            }
        }
    }
    public void RegisterArtifactCollection(QuestData quest, string artifactId)
    {
        if (activeQuests.Contains(quest) && !quest.isCompleted)
        {
            quest.currentKills++; // ✅ Simule la progression d’objectif
            Debug.Log($"📜 Artéfact collecté pour la quête : {quest.questTitle} ({quest.currentKills}/{quest.requiredKills})");

            if (quest.currentKills >= quest.requiredKills)
            {
                CompleteQuest(quest);
            }
        }
    }
    public static void ResetAll()
    {
        if (Instance != null)
        {
            Instance.activeQuests.Clear();
            Instance.collectedArtifacts.Clear();
            Debug.Log("📘 QuestManager réinitialisé.");
        }
    }

}
