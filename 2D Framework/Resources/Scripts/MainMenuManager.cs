using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private bool showOptions = false;
    private Rect menuRect;
    private Vector2 panelSize = new Vector2(320, 300);
    private bool musicOn = true;
    private float volume = 1f;
    private bool fullscreen = true;

    private void Start()
    {
        // Centrage du menu
        float x = (Screen.width - panelSize.x) / 2f;
        float y = (Screen.height - panelSize.y) / 2f;
        menuRect = new Rect(x, y, panelSize.x, panelSize.y);
    }

    private void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 14
        };

        GUI.Box(menuRect, ""); // Fond
        GUI.Label(new Rect(menuRect.x, menuRect.y + 10, menuRect.width, 30), "⛨ Eldrya: Knight's Dawn ⛨", titleStyle);

        float btnWidth = 200;
        float btnHeight = 40;
        float spacing = 12;
        float startY = menuRect.y + 60;
        float centerX = menuRect.x + (menuRect.width - btnWidth) / 2f;

        float currentY = startY;

        // 🆕 Nouvelle Partie
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), "🆕 Nouvelle Partie", buttonStyle))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("ActiveCam", "FixedCam_SpawnZone");
            SceneManager.LoadScene("Game");
        }
        currentY += btnHeight + spacing;

        // ▶ Charger la partie
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), "▶ Charger la partie", buttonStyle))
        {
            SceneManager.LoadScene("Game");
        }
        currentY += btnHeight + spacing;

        // ⚙ Options
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), showOptions ? "▲ Hide Options" : "⚙ Options", buttonStyle))
        {
            showOptions = !showOptions;
        }
        currentY += btnHeight + spacing;

        // ✖ Quit
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), "✖ Quit", buttonStyle))
        {
            Application.Quit();
            Debug.Log("Quit game triggered.");
        }


        if (showOptions)
        {
            Rect optionRect = new Rect(centerX, startY + (btnHeight + spacing) * 3 + 10, btnWidth, 160);
            GUI.Box(optionRect, "Options");

            GUI.Label(new Rect(optionRect.x + 10, optionRect.y + 30, 120, 20), "🎵 Music:");
            musicOn = GUI.Toggle(new Rect(optionRect.x + 100, optionRect.y + 30, 20, 20), musicOn, "");

            GUI.Label(new Rect(optionRect.x + 10, optionRect.y + 60, 120, 20), "🔊 Volume:");
            volume = GUI.HorizontalSlider(new Rect(optionRect.x + 80, optionRect.y + 65, 100, 20), volume, 0f, 1f);

            GUI.Label(new Rect(optionRect.x + 10, optionRect.y + 90, 120, 20), "🖥 Fullscreen:");
            fullscreen = GUI.Toggle(new Rect(optionRect.x + 100, optionRect.y + 90, 20, 20), fullscreen, "");

            // Apply Settings
            if (GUI.Button(new Rect(optionRect.x + 10, optionRect.y + 120, optionRect.width - 20, 25), "Apply"))
            {
                AudioListener.volume = volume;
                Screen.fullScreen = fullscreen;
            }
        }
    }
}
