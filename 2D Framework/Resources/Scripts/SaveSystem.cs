using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public void SaveGame(PlayerController player, InventoryGUI inventory, LevelSystem level)
    {
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);

        PlayerPrefs.SetInt("Health", player.currentHealth);
        PlayerPrefs.SetInt("Mana", (int)player.currentMana);
        PlayerPrefs.SetInt("Endurance", (int)player.currentEndurance);

        PlayerPrefs.SetInt("XP", level.currentExperience);
        PlayerPrefs.SetInt("Level", level.currentLevel);
        PlayerPrefs.SetInt("XPNext", level.experienceToNextLevel);

        if (CameraFixedZoneController.Instance != null)
        {
            var cam = CameraFixedZoneController.Instance.mainCamera;
            var point = CameraFixedZoneController.Instance.currentCamPoint;

            if (cam != null)
                PlayerPrefs.SetString("ActiveCam", cam.name);

            if (point != null)
                PlayerPrefs.SetString("SavedCamPoint", point.name);
        }

        for (int i = 0; i < inventory.items.Count; i++)
        {
            PlayerPrefs.SetString($"Inventory_{i}", inventory.items[i] != null ? inventory.items[i].itemID : "");
        }
        for (int i = 0; i < inventory.equipmentSlots.Length; i++)
        {
            PlayerPrefs.SetString($"Equip_{i}", inventory.equipmentSlots[i].itemData != null ? inventory.equipmentSlots[i].itemData.itemID : "");
        }
        for (int i = 0; i < inventory.actionBarSlots.Length; i++)
        {
            PlayerPrefs.SetString($"Action_{i}", inventory.actionBarSlots[i].item != null ? inventory.actionBarSlots[i].item.itemID : "");
        }
    }

    public void LoadGame(PlayerController player, InventoryGUI inventory, LevelSystem level, ItemDatabase itemDB)
    {
        float x = PlayerPrefs.GetFloat("PlayerX", player.transform.position.x);
        float y = PlayerPrefs.GetFloat("PlayerY", player.transform.position.y);
        player.transform.position = new Vector2(x, y);

        player.currentHealth = PlayerPrefs.GetInt("Health", player.maxHealth);
        player.currentMana = PlayerPrefs.GetInt("Mana", (int)player.maxMana);
        player.currentEndurance = PlayerPrefs.GetInt("Endurance", (int)player.maxEndurance);

        player.UpdateHealthBar();
        player.UpdateManaBar();
        player.UpdateEnduranceBar();

        level.currentExperience = PlayerPrefs.GetInt("XP", 0);
        level.currentLevel = PlayerPrefs.GetInt("Level", 1);
        level.experienceToNextLevel = PlayerPrefs.GetInt("XPNext", 100);
        level.experienceBar.value = (float)level.currentExperience / level.experienceToNextLevel;

        for (int i = 0; i < inventory.items.Count; i++)
        {
            string id = PlayerPrefs.GetString($"Inventory_{i}", "");
            // inventory.items[i] = itemDB.GetItemByID(id);
        }
        for (int i = 0; i < inventory.equipmentSlots.Length; i++)
        {
            string id = PlayerPrefs.GetString($"Equip_{i}", "");
            // inventory.equipmentSlots[i].itemData = itemDB.GetItemByID(id);
        }
        for (int i = 0; i < inventory.actionBarSlots.Length; i++)
        {
            string id = PlayerPrefs.GetString($"Action_{i}", "");
            // inventory.actionBarSlots[i].item = itemDB.GetItemByID(id);
        }
        // ✅ Réactivation explicite de la caméra
        string camPointName = PlayerPrefs.GetString("SavedCamPoint", "");
        if (!string.IsNullOrEmpty(camPointName))
        {
            Transform camPoint = GameObject.Find(camPointName)?.transform;

            // ✅ Fallback automatique si caméra invalide
            if (camPoint == null)
            {
                Debug.LogWarning("⚠ Caméra introuvable, fallback sur FixedCam_SpawnZone.");
                camPoint = GameObject.Find("FixedCam_SpawnZone")?.transform;
                PlayerPrefs.SetString("SavedCamPoint", "FixedCam_SpawnZone"); // on corrige aussi
            }

            if (CameraFixedZoneController.Instance != null && camPoint != null)
            {
                CameraFixedZoneController.Instance.MoveCameraTo(camPoint);
                if (CameraFixedZoneController.Instance.mainCamera != null)
                    CameraFixedZoneController.Instance.mainCamera.gameObject.SetActive(true);
            }

        }


        Debug.Log("✅ Partie chargée !");
    }
}
