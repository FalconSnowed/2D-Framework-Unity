using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public static bool GameStarted = false;

    [Header("Références essentielles")]
    public Transform playerSpawnPoint;
    public Transform cameraStartPoint;
    public AudioClip introMusic;
    public GameObject uiCanvas;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (GameStarted) return; // Ne relance pas si déjà initialisé
        GameStarted = true;

        // 💾 Reset des données statiques ou managers si nécessaire
        QuestManager.ResetAll(); // Exemple
        LevelSystem.ResetStats(); // Exemple

        // 👤 Spawn joueur
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        player.tag = "Player";

        // 🎥 Caméra
        if (CameraFixedZoneController.Instance != null)
        {
            CameraFixedZoneController.Instance.MoveCameraTo(cameraStartPoint);
        }

        // 🎶 Musique
        if (MusicManager.Instance != null && introMusic != null)
        {
            MusicManager.Instance.musicSource.clip = introMusic;
            MusicManager.Instance.musicSource.Play();
        }

        // 🎮 Interface UI
        if (uiCanvas != null)
            uiCanvas.SetActive(true);
    }
}
