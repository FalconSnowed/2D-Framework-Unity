using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class LevelSystem : NetworkBehaviour
{
    // Champs publics classiques (compatibles avec ton SaveSystem)
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;

    public Slider experienceBar;

    // Réseau (si besoin de synchro visible)
    [Networked] private int SyncLevel { get; set; }
    [Networked] private int SyncExperience { get; set; }
    [Networked] private int SyncXPToNext { get; set; }

    public override void Spawned()
    {
        // Initialise la synchro à partir des valeurs locales
        if (HasStateAuthority)
        {
            SyncLevel = currentLevel;
            SyncExperience = currentExperience;
            SyncXPToNext = experienceToNextLevel;
        }
    }

    public void AddExperience(int amount)
    {
        if (!HasStateAuthority) return;

        currentExperience += amount;

        if (currentExperience >= experienceToNextLevel)
            LevelUp();

        UpdateXPBar();
        SyncState();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

        Debug.Log("⚡ Level Up! Now: " + currentLevel);
    }

    private void UpdateXPBar()
    {
        if (experienceBar != null)
            experienceBar.value = (float)currentExperience / experienceToNextLevel;
    }

    private void SyncState()
    {
        SyncLevel = currentLevel;
        SyncExperience = currentExperience;
        SyncXPToNext = experienceToNextLevel;
    }

    public override void FixedUpdateNetwork()
    {
        // Si on est un client, on lit la version réseau
        if (!HasStateAuthority)
        {
            currentLevel = SyncLevel;
            currentExperience = SyncExperience;
            experienceToNextLevel = SyncXPToNext;
            UpdateXPBar();
        }
    }

    public static void ResetStats()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent(out LevelSystem level))
        {
            if (!level.HasStateAuthority) return;

            level.currentLevel = 1;
            level.currentExperience = 0;
            level.experienceToNextLevel = 100;

            if (level.experienceBar != null)
                level.experienceBar.value = 0f;

            Debug.Log("📈 Système de niveau réinitialisé.");
        }
    }
}
