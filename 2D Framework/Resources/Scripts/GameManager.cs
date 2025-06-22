using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsInventoryOpen = false;

    void Start()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        InventoryGUI inventory = FindObjectOfType<InventoryGUI>();
        LevelSystem level = FindObjectOfType<LevelSystem>();
        ItemDatabase itemDB = FindObjectOfType<ItemDatabase>();
        SaveSystem saveSystem = FindObjectOfType<SaveSystem>();

        if (player != null && inventory != null && level != null && itemDB != null && saveSystem != null)
        {
            saveSystem.LoadGame(player, inventory, level, itemDB);
        }
        else
        {
            Debug.LogWarning("❗ Impossible de charger la partie : un ou plusieurs éléments sont manquants.");
        }
    }



    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
