using UnityEngine;
using System.Collections;

public class TPWorldTriggerTwo : MonoBehaviour
{
    public Transform teleportDestination;
    public Transform cameraPointForZone;

    [Header("Audio par zone")]
    public int destinationZoneIndex;
    public AudioClip zone0MusicClip;
    public AudioClip zone1MusicClip;
    public AudioClip zone2MusicClip;

    [Header("Transition")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;
    public string zoneCinematicMessage = "Une brume étrange vous enveloppe...";
    public float messageDisplayTime = 3f;

    private bool isTransitioning = false;
    private bool showMessage = false;
    private float messageTimer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning || !other.CompareTag("Player")) return;

        StartCoroutine(PlayZoneTransition(other.gameObject));
    }

    private IEnumerator PlayZoneTransition(GameObject player)
    {
        isTransitioning = true;

        // 🔒 Stop player movement
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.canMove = false;

        // 🎬 Fade-out
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0, 1));

        // 🧭 Téléportation
        player.transform.position = teleportDestination.position;

        // 📷 Caméra
        if (CameraFixedZoneController.Instance != null)
            CameraFixedZoneController.Instance.MoveCameraTo(cameraPointForZone);

        // 🎵 Musique
        PlayZoneMusic();

        // 🧙‍♂️ Affichage texte cinématique
        showMessage = true;
        messageTimer = messageDisplayTime;
        yield return new WaitForSeconds(messageDisplayTime);

        // 🎬 Fade-in
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1, 0));

        // 🔓 Reactivate player
        if (controller != null)
            controller.canMove = true;

        showMessage = false;
        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            float normalized = t / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
            t += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = endAlpha;
    }

    private void PlayZoneMusic()
    {
        if (MusicManager.Instance == null) return;

        AudioClip clipToPlay = null;
        switch (destinationZoneIndex)
        {
            case 0: clipToPlay = zone0MusicClip; break;
            case 1: clipToPlay = zone1MusicClip; break;
            case 2: clipToPlay = zone2MusicClip; break;
        }

        if (clipToPlay != null && MusicManager.Instance.musicSource.clip != clipToPlay)
        {
            MusicManager.Instance.musicSource.clip = clipToPlay;
            MusicManager.Instance.musicSource.Play();
        }
    }

    private void OnGUI()
    {
        if (showMessage && !string.IsNullOrEmpty(zoneCinematicMessage))
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                normal = { textColor = Color.white }
            };

            Rect rect = new Rect(0, Screen.height / 2 - 50, Screen.width, 100);
            GUI.Label(rect, zoneCinematicMessage, style);
        }
    }

    private void Update()
    {
        if (showMessage)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
                showMessage = false;
        }
    }
}
