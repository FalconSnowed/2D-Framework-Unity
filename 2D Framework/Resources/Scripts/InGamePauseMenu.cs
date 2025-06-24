﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class InGamePauseMenu : MonoBehaviour
{
    private bool showOptions = false;
    private bool introActive = true;
    private bool showMenu = false;
    private float menuAlpha = 0f;

    private Vector2 menuSize = new Vector2(420, 500);

    private bool musicOn = true;
    private float volume = 1f;
    private bool fullscreen = true;

    private PlayerController player;
    private InventoryGUI inventory;
    private LevelSystem level;
    private ItemDatabase itemDB;
    private SaveSystem saveSystem;

    [System.Obsolete]
    private void Start()
    {
        float x = (Screen.width - menuSize.x) / 2f;
        float y = (Screen.height - menuSize.y) / 2f;

        // Récupère uniquement le joueur local (celui avec Input Authority)
        foreach (var p in FindObjectsOfType<PlayerController>())
        {
            if (p.Object != null && p.HasInputAuthority)
            {
                player = p;
                break;
            }
        }

        inventory = FindObjectOfType<InventoryGUI>();
        level = FindObjectOfType<LevelSystem>();
        itemDB = FindObjectOfType<ItemDatabase>();
        saveSystem = FindObjectOfType<SaveSystem>();

        if (player && inventory && level && itemDB && saveSystem)
        {
            saveSystem.LoadGame(player, inventory, level, itemDB);
        }
        else
        {
            Debug.LogWarning("❗ Impossible de charger la partie : un ou plusieurs composants manquants.");
        }

        Invoke(nameof(EndIntro), 8f);
    }

    private void EndIntro() => introActive = false;

    private void Update()
    {
        if (!introActive && Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu = !showMenu;
            menuAlpha = showMenu ? 0f : 1f;
        }

        if (showMenu && menuAlpha < 1f)
            menuAlpha += Time.deltaTime * 2f;
        else if (!showMenu && menuAlpha > 0f)
            menuAlpha -= Time.deltaTime * 2f;
    }

    private void OnGUI()
    {
        if (introActive || menuAlpha <= 0f || player == null) return;

        GUI.color = new Color(0, 0, 0, 0.6f * menuAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        Rect fullRect = new Rect((Screen.width - menuSize.x) / 2f, (Screen.height - menuSize.y) / 2f, menuSize.x, menuSize.y);
        GUIStyle menuBoxStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            padding = new RectOffset(12, 12, 12, 12)
        };

        GUI.Box(fullRect, "⏸ Eldora – Echo of Time", menuBoxStyle);

        float btnWidth = 280;
        float btnHeight = 45;
        float spacing = 14;
        float currentY = fullRect.y + 60;
        float centerX = fullRect.x + (fullRect.width - btnWidth) / 2f;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 15
        };

        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), "🆕 Nouvelle Partie", buttonStyle))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("ActiveCam", "FixedCam_SpawnZone");
            PlayerPrefs.SetInt("IntroPlayed", 0);
            SceneManager.LoadScene("Game");
        }

        currentY += btnHeight + spacing;
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), showOptions ? "▲ Masquer Options" : "⚙ Options", buttonStyle))
            showOptions = !showOptions;

        currentY += btnHeight + spacing;
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), "⏯ Reprendre", buttonStyle))
            showMenu = false;

        currentY += btnHeight + spacing;
        if (GUI.Button(new Rect(centerX, currentY, btnWidth, btnHeight), "❌ Quitter le jeu", buttonStyle))
            Application.Quit();

        if (showOptions)
        {
            Rect opt = new Rect(centerX, currentY + 30, btnWidth, 160);
            GUI.Box(opt, "🔧 Paramètres");

            musicOn = GUI.Toggle(new Rect(opt.x + 10, opt.y + 30, 200, 20), musicOn, " 🎵 Musique");
            GUI.Label(new Rect(opt.x + 10, opt.y + 60, 80, 20), "Volume:");
            volume = GUI.HorizontalSlider(new Rect(opt.x + 90, opt.y + 65, 100, 20), volume, 0f, 1f);
            fullscreen = GUI.Toggle(new Rect(opt.x + 10, opt.y + 90, 200, 20), fullscreen, " 🖥️ Plein écran");

            if (GUI.Button(new Rect(opt.x + 10, opt.y + 120, 260, 25), "✅ Appliquer"))
            {
                AudioListener.volume = volume;
                Screen.fullScreen = fullscreen;
            }
        }

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            normal = { textColor = Color.yellow }
        };

        GUI.Label(new Rect(fullRect.x - 180, fullRect.y + 60, 180, 200),
            $"👤 {player?.playerName ?? "Inconnu"}\n🧱 Classe: {player?.className}\n❤️ Vie: {player?.currentHealth}/{player?.maxHealth}\n🌎 Zone: {PlayerPrefs.GetString("SavedCamPoint", "???")}", labelStyle);
    }
}
