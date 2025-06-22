// AOEHUD.cs (à attacher à un GameObject de type GUI ou dans un GameManager unique)
using UnityEngine;

public class AOEHUD : MonoBehaviour
{
    public PlayerSpells playerSpells;
    public float cooldownDuration = 5f;
    private float cooldownTimer = 0f;

    private void Update()
    {
        if (playerSpells == null) return;

        if (playerSpells.IsAOEOnCooldown())
            cooldownTimer = playerSpells.RemainingCooldown();
        else
            cooldownTimer = 0f;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        style.normal.textColor = cooldownTimer > 0 ? Color.red : Color.green;

        string label = cooldownTimer > 0
            ? $"AOE: {cooldownTimer:F1}s"
            : "AOE: READY";

        GUI.Label(new Rect(10, 40, 200, 30), label, style);
    }
}
