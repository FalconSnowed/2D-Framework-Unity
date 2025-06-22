using UnityEngine;

[System.Serializable]
public class EquipmentSlot
{
    public string slotName; // ex: "Helmet", "Weapon"
    public Texture2D icon;  // icône de l’objet équipé
    public ItemData itemData;
}
