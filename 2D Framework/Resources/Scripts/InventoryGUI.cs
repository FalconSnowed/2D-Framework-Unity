using UnityEngine;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour
{
    public Texture2D slotTexture;
    public List<ItemData> items;
    public int columns = 4;
    public int totalSlots = 12;
    public Vector2 slotSize = new Vector2(64, 64);
    public int spacing = 4;
    public Texture2D inventoryBackground; public EquipmentSlot[] equipmentSlots = new EquipmentSlot[3]; // Helmet, Torso, Weapon
    public Vector2 equipSlotSize = new Vector2(48, 48);
    public Texture2D equipSlotTexture;
    private ItemData selectedItem = null;
    public PlayerController player;
    public ActionBarSlot[] actionBarSlots = new ActionBarSlot[4];
    public Vector2 actionBarSlotSize = new Vector2(48, 48);
    public Vector2 actionBarPosition = new Vector2(10, Screen.height - 64);
    public float itemUseCooldown = 2f; // Temps entre utilisations
    public LevelSystem levelSystem;

    private bool showInventory = false;

    private void Start()
    {
        if (actionBarSlots[0] == null)
        {
            actionBarSlots[0] = new ActionBarSlot { hotkey = KeyCode.Alpha1 };
            actionBarSlots[1] = new ActionBarSlot { hotkey = KeyCode.Alpha2 };
            actionBarSlots[2] = new ActionBarSlot { hotkey = KeyCode.Alpha3 };
            actionBarSlots[3] = new ActionBarSlot { hotkey = KeyCode.Alpha4 };
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            showInventory = !showInventory;
            GameManager.Instance.IsInventoryOpen = showInventory;
        }
        if (GameManager.Instance != null)
            GameManager.Instance.IsInventoryOpen = showInventory;
        for (int i = 0; i < actionBarSlots.Length; i++)
        {
            var slot = actionBarSlots[i];
            if (Input.GetKeyDown(slot.hotkey) && slot.item != null && Time.time - slot.lastUseTime > itemUseCooldown)
            {
                if (slot.item.itemType == ItemData.ItemType.Potion && player.currentHealth < player.maxHealth)
                {
                    player.currentHealth = (int)Mathf.Min(player.currentHealth + slot.item.healAmount, player.maxHealth);
                    player.UpdateHealthBar();
                    slot.item = null;
                    slot.lastUseTime = Time.time;
                }
            }
        }
    }

    private ItemData GetItemUnderMouse(Vector2 mousePos)
    {
        // Inventaire
        int rows = Mathf.CeilToInt((float)totalSlots / columns);
        float startX = (Screen.width - ((columns * slotSize.x + (columns - 1) * spacing) + equipSlotSize.x + 64f + 16f * 2)) / 2f + equipSlotSize.x + 64f;
        float startY = (Screen.height - rows * slotSize.y + (rows - 1) * spacing) / 2f;

        for (int i = 0; i < totalSlots; i++)
        {
            int row = i / columns;
            int col = i % columns;

            float x = startX + (slotSize.x + spacing) * col;
            float y = startY + (slotSize.y + spacing) * row;
            Rect slotRect = new Rect(x, y, slotSize.x, slotSize.y);

            if (slotRect.Contains(mousePos) && i < items.Count)
                return items[i];
        }

        // Barre d’action
        for (int i = 0; i < actionBarSlots.Length; i++)
        {
            float x = actionBarPosition.x + i * (actionBarSlotSize.x + spacing);
            float y = actionBarPosition.y;
            Rect slotRect = new Rect(x, y, actionBarSlotSize.x, actionBarSlotSize.y);

            if (slotRect.Contains(mousePos))
                return actionBarSlots[i].item;
        }

        return null;
    }

    private void OnGUI()
    {
        // ✅ Action Bar : TOUJOURS visible (même si l'inventaire est fermé)
        DrawActionBar();

        // ✅ Inventaire : visible uniquement si ouvert
        if (!showInventory)
            return;

        DrawInventory();
        // 🔍 INFO-BULLE TOOLTIP
        Vector2 mousePos = Event.current.mousePosition;
        ItemData hoveredItem = GetItemUnderMouse(mousePos);
        if (hoveredItem != null)
        {
            float boxWidth = 160;
            float boxHeight = 80;

            Rect tooltipRect = new Rect(mousePos.x + 20, mousePos.y, boxWidth, boxHeight);
            GUI.Box(tooltipRect, "");

            GUI.Label(new Rect(tooltipRect.x + 8, tooltipRect.y + 8, boxWidth - 16, 20), hoveredItem.itemName);
            GUI.Label(new Rect(tooltipRect.x + 8, tooltipRect.y + 30, boxWidth - 16, 20), hoveredItem.itemType.ToString());

            if (hoveredItem.itemType == ItemData.ItemType.Potion)
                GUI.Label(new Rect(tooltipRect.x + 8, tooltipRect.y + 50, boxWidth - 16, 20), $"Soin : {hoveredItem.healAmount}");
            else if (hoveredItem.itemType == ItemData.ItemType.Equipment)
                GUI.Label(new Rect(tooltipRect.x + 8, tooltipRect.y + 50, boxWidth - 16, 20), $"+ATK : {hoveredItem.attackBonus} / +DEF : {hoveredItem.defenseBonus}");
        }

    }

    private void DrawInventory()
    {
        int rows = Mathf.CeilToInt((float)totalSlots / columns);
        float inventoryWidth = columns * slotSize.x + (columns - 1) * spacing;
        float inventoryHeight = rows * slotSize.y + (rows - 1) * spacing;

        float padding = 16f;
        float sidePanelWidth = 170f;

        float startX = (Screen.width - (inventoryWidth + sidePanelWidth + padding * 2)) / 2f + sidePanelWidth;
        float startY = (Screen.height - inventoryHeight) / 2f;

        Rect bgRect = new Rect(startX - sidePanelWidth - padding, startY - padding,
            inventoryWidth + sidePanelWidth + padding * 2,
            inventoryHeight + padding * 2);

        if (inventoryBackground != null)
            GUI.DrawTexture(bgRect, inventoryBackground, ScaleMode.StretchToFill);
        else
            GUI.Box(bgRect, "Inventory & Equipment");

        GUI.Label(new Rect(bgRect.x, bgRect.y, bgRect.width, 20), "Inventory & Equipment", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperCenter,
            fontStyle = FontStyle.Bold
        });

        float leftX = bgRect.x + padding;
        float currentY = startY + 10f;

        // 🔹 Équipement
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            Rect equipRect = new Rect(leftX, currentY, equipSlotSize.x, equipSlotSize.y);
            GUI.DrawTexture(equipRect, equipSlotTexture != null ? equipSlotTexture : Texture2D.whiteTexture);

            if (equipmentSlots[i].icon != null)
                GUI.DrawTexture(equipRect, equipmentSlots[i].icon, ScaleMode.ScaleToFit);

            GUI.Label(new Rect(leftX + equipSlotSize.x + 6, currentY + 12, 70, 20), equipmentSlots[i].slotName);
            currentY += equipSlotSize.y + 24f; // 🔧 plus d'espace entre les slots
        }

        // 🔹 Portrait
        if (player.portrait != null)
        {
            currentY += 10f;
            Rect portraitRect = new Rect(leftX, currentY, 64, 64);
            GUI.DrawTexture(portraitRect, player.portrait, ScaleMode.ScaleToFit);
            currentY += 72f;
        }

        // 🔹 Infos joueur
        GUI.Label(new Rect(leftX, currentY, 150, 20), $"Nom : {player.playerName}"); currentY += 20;
        GUI.Label(new Rect(leftX, currentY, 150, 20), $"Classe : {player.className}"); currentY += 20;
        GUI.Label(new Rect(leftX, currentY, 150, 20), $"Niveau : {levelSystem.currentLevel}"); currentY += 28;

        // 🔹 Barre XP
        float xpRatio = (float)levelSystem.currentExperience / levelSystem.experienceToNextLevel;
        GUI.Box(new Rect(leftX, currentY, 100, 12), "");
        GUI.Box(new Rect(leftX, currentY, 100 * xpRatio, 12), "", GUIStyle.none);
        GUI.Label(new Rect(leftX, currentY + 12, 120, 20), $"XP : {levelSystem.currentExperience}/{levelSystem.experienceToNextLevel}"); currentY += 34;

        // 🔹 Stats
        GUI.Label(new Rect(leftX, currentY, 150, 20), $"HP : {player.currentHealth}/{player.maxHealth}"); currentY += 20;
        GUI.Label(new Rect(leftX, currentY, 150, 20), $"ATK : {player.attack}"); currentY += 20;
        GUI.Label(new Rect(leftX, currentY, 150, 20), $"DEF : {player.defense}");


        // Slots
        for (int i = 0; i < totalSlots; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float x = startX + (slotSize.x + spacing) * col;
            float y = startY + (slotSize.y + spacing) * row;
            Rect slotRect = new Rect(x, y, slotSize.x, slotSize.y);

            if (Event.current.type == EventType.MouseDown)
            {
                if (slotRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0)
                    {
                        if (selectedItem == null && i < items.Count && items[i] != null)
                        {
                            selectedItem = items[i];
                            items[i] = null;
                            Event.current.Use();
                        }
                        else if (selectedItem != null && (i >= items.Count || items[i] == null))
                        {
                            if (i >= items.Count) while (items.Count <= i) items.Add(null);
                            items[i] = selectedItem;
                            selectedItem = null;
                            Event.current.Use();
                        }
                    }
                    else if (Event.current.button == 1 && i < items.Count && items[i] != null && items[i].itemType == ItemData.ItemType.Potion)
                    {
                        if (player.currentHealth < player.maxHealth)
                        {
                            player.currentHealth = (int)Mathf.Min(player.currentHealth + items[i].healAmount, player.maxHealth);
                            player.UpdateHealthBar();
                            items[i] = null;
                            Event.current.Use();
                        }
                    }
                }
            }

            GUI.DrawTexture(slotRect, slotTexture != null ? slotTexture : Texture2D.grayTexture);
            if (i < items.Count && items[i]?.icon != null)
                GUI.DrawTexture(slotRect, items[i].icon, ScaleMode.ScaleToFit);
        }

        if (selectedItem != null && selectedItem.icon != null)
        {
            Rect followMouse = new Rect(Event.current.mousePosition.x - 16, Event.current.mousePosition.y - 16, 32, 32);
            GUI.DrawTexture(followMouse, selectedItem.icon, ScaleMode.ScaleToFit);
        }
    }



    private void DrawActionBar()
    {
        for (int i = 0; i < actionBarSlots.Length; i++)
        {
            float x = actionBarPosition.x + i * (actionBarSlotSize.x + spacing);
            float y = actionBarPosition.y;
            Rect slotRect = new Rect(x, y, actionBarSlotSize.x, actionBarSlotSize.y);
            GUI.Box(slotRect, (i + 1).ToString());

            if (actionBarSlots[i].item != null && actionBarSlots[i].item.icon != null)
            {
                GUI.DrawTexture(slotRect, actionBarSlots[i].item.icon, ScaleMode.ScaleToFit);
            }

            // ✅ Drag depuis inventaire
            if (slotRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && selectedItem != null)
                {
                    actionBarSlots[i].item = selectedItem;
                    selectedItem = null;
                    Event.current.Use();
                }

                // ✅ Supprimer avec clic droit
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    actionBarSlots[i].item = null;
                    Event.current.Use();
                }
            }
        }
    }

}
