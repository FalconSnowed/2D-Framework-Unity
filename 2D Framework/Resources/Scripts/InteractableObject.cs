using UnityEngine;
using Fusion;

public class InteractableObject : NetworkBehaviour
{
    private bool isPlayerInRange;
    private bool showGUI = false;
    private NetworkObject playerInTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var no = collision.GetComponent<NetworkObject>();
            if (no != null && no.HasInputAuthority)
            {
                isPlayerInRange = true;
                showGUI = true;
                playerInTrigger = no;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var no = collision.GetComponent<NetworkObject>();
            if (no != null && no.HasInputAuthority)
            {
                isPlayerInRange = false;
                showGUI = false;
                playerInTrigger = null;
            }
        }
    }

    private void OnGUI()
    {
        if (!showGUI || playerInTrigger == null) return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        GUILayout.BeginArea(new Rect(screenPos.x - 60, Screen.height - screenPos.y - 80, 120, 100), GUI.skin.box);

        if (GUILayout.Button("Inspecter")) Inspect();
        if (GUILayout.Button("Prendre")) Prendre();
        if (GUILayout.Button("Annuler")) showGUI = false;

        GUILayout.EndArea();
    }

    private void Inspect()
    {
        Debug.Log("[Fusion] Inspection de l’objet par le joueur local.");
        showGUI = false;
    }
    public Texture2D itemIcon; // à glisser dans l’Inspector

    private void Prendre()
    {
        Debug.Log("[Fusion] Objet pris !");
        showGUI = false;

        if (playerInTrigger == null) return;

        // Recherche le script InventoryGUI du joueur local
        InventoryGUI inventory = playerInTrigger.GetComponentInChildren<InventoryGUI>();
        if (inventory == null)
        {
            Debug.LogWarning("Aucun InventoryGUI trouvé sur le joueur.");
            return;
        }

        // Simule un objet récupéré (exemple générique à remplacer si nécessaire)
        ItemData newItem = ScriptableObject.CreateInstance<ItemData>();
        newItem.itemName = "Fragment Atomique";
        newItem.itemType = ItemData.ItemType.Equipment;
        newItem.attackBonus = 2;
        newItem.defenseBonus = 1;
        newItem.icon = itemIcon;

        // Ajoute l’objet dans le premier slot libre
        bool added = false;
        for (int i = 0; i < inventory.totalSlots; i++)
        {
            if (i >= inventory.items.Count)
                inventory.items.Add(null);

            if (inventory.items[i] == null)
            {
                inventory.items[i] = newItem;
                added = true;
                break;
            }
        }

        if (added)
        {
            Debug.Log("Objet ajouté à l’inventaire !");
            if (Object.HasStateAuthority)
                Runner.Despawn(Object); // Supprime l’objet dans le monde
        }
        else
        {
            Debug.Log("Inventaire plein !");
        }
    }
}
