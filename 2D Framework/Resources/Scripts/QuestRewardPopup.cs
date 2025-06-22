using UnityEngine;

public class QuestRewardPopup : MonoBehaviour
{
    private string message = "";
    private bool visible = false;
    private float timer = 0f;
    public Vector2 position = new Vector2(20, 100);
    public GUIStyle popupStyle;

    public void Show(string msg, float duration = 3f)
    {
        message = msg;
        visible = true;
        timer = duration;
    }

    void Update()
    {
        if (visible)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                visible = false;
        }
    }

    void OnGUI()
    {
        if (!visible) return;

        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.box);
            popupStyle.fontSize = 14;
            popupStyle.normal.textColor = Color.yellow;
            popupStyle.alignment = TextAnchor.MiddleCenter;
            popupStyle.fontStyle = FontStyle.Bold;
        }

        Rect rect = new Rect(position.x, position.y, 320, 30);
        GUI.Box(rect, message, popupStyle);
    }
}
