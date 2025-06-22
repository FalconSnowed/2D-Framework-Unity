using UnityEngine;
using System.Collections.Generic;

public class QuestGUI : MonoBehaviour
{
    public Vector2 windowPosition = new Vector2(20, 20);
    public Vector2 windowSize = new Vector2(300, 400);
    private Vector2 scrollPos;

    private void OnGUI()
    {
        if (QuestManager.Instance == null || QuestManager.Instance.activeQuests == null)
            return;

        // 🔹 Style de fenêtre transparent
        GUIStyle transparentStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = Texture2D.blackTexture }, // texture noire unie
            fontSize = 14,
            richText = true,
            alignment = TextAnchor.UpperCenter
        };

        Color originalColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.25f); // fond noir avec alpha 25%

        // ✅ Fenêtre transparente simulée
        GUI.Box(new Rect(windowPosition.x, windowPosition.y, windowSize.x, windowSize.y), "Quêtes Actives", transparentStyle);

        GUI.color = originalColor; // reset couleur pour le texte normal

        Rect scrollRect = new Rect(windowPosition.x + 10, windowPosition.y + 30, windowSize.x - 20, windowSize.y - 40);
        scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, new Rect(0, 0, scrollRect.width - 20, QuestManager.Instance.activeQuests.Count * 80));

        int y = 0;
        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            string status = quest.isCompleted ? "<color=green>[Terminé]</color>" : "<color=red>[En cours]</color>";
            GUI.Label(new Rect(0, y, scrollRect.width - 20, 20), $"<b>{quest.questTitle}</b> {status}", GetRichStyle());
            GUI.Label(new Rect(0, y + 20, scrollRect.width - 20, 60), quest.questDescription, GetWrappedStyle());
            y += 80;
        }

        GUI.EndScrollView();
    }


    private GUIStyle GetWrappedStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
            fontSize = 12
        };
    }

    private GUIStyle GetRichStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            richText = true,
            fontStyle = FontStyle.Bold
        };
    }
}
