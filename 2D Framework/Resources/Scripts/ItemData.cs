using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item (Texture2D)")]
public class ItemData : ScriptableObject
{

    public string itemName;
    public Texture2D icon;
    public ItemType itemType;
    public string itemID;
    // Pour potions
    public int healAmount;

    // Pour équipements
    public int attackBonus;
    public int defenseBonus;

    public enum ItemType
    {
        Potion,
        Equipment
    }
}
