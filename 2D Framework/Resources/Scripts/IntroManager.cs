using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EpicIntroManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip introMusic;
    public float musicFadeInTime = 2f;
    [Header("Camera")]
    public Camera introCamera;
    [Header("UI")]
    public CanvasGroup fadeCanvas;
    public GameObject[] chatBubbles;
    public float bubbleFadeTime = 0.5f;
    public float bubbleDelay = 2f;

    [Header("Options")]
    public bool allowSkip = true;
    public KeyCode skipKey = KeyCode.Escape;
    public AudioClip mainThemeMusic;
    public float musicTransitionDelay = 1f;
    public float musicFadeOutTime = 1f;

    private bool skipped = false;
    private bool hasStarted = false;

    public static event System.Action OnIntroFinished;

    private void Start()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(PlayIntroSequence());
        }
    }

    private IEnumerator PlayIntroSequence()
    {
        // Fade from black
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 2f;
                fadeCanvas.alpha = 1f - Mathf.Clamp01(t);
                yield return null;
            }
        }

        // Intro music fade-in
        if (musicSource != null && introMusic != null)
        {
            musicSource.clip = introMusic;
            musicSource.volume = 0f;
            musicSource.Play();

            float t = 0f;
            while (t < musicFadeInTime)
            {
                t += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0f, 1f, t / musicFadeInTime);
                yield return null;
            }
        }

        // Bubble sequence
        for (int i = 0; i < chatBubbles.Length; i++)
        {
            if (skipped) break;

            GameObject currentBubble = chatBubbles[i];
            if (currentBubble == null) continue;

            CanvasGroup cg = currentBubble.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                Debug.LogWarning($"[Intro] Bubble {i} has no CanvasGroup.");
                continue;
            }

            currentBubble.SetActive(true);
            cg.alpha = 0f;
            currentBubble.transform.localScale = Vector3.one * 0.8f;

            float appearTime = 0f;
            while (appearTime < bubbleFadeTime)
            {
                appearTime += Time.deltaTime;
                float lerp = appearTime / bubbleFadeTime;
                cg.alpha = Mathf.Lerp(0f, 1f, lerp);
                currentBubble.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, lerp);
                yield return null;
            }

            cg.alpha = 1f;
            currentBubble.transform.localScale = Vector3.one;

            float wait = 0f;
            while (wait < bubbleDelay && !skipped)
            {
                wait += Time.deltaTime;
                yield return null;
            }

            float disappearTime = 0f;
            while (disappearTime < bubbleFadeTime)
            {
                disappearTime += Time.deltaTime;
                float lerp = disappearTime / bubbleFadeTime;
                cg.alpha = Mathf.Lerp(1f, 0f, lerp);
                currentBubble.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, lerp);
                yield return null;
            }

            cg.alpha = 0f;
            currentBubble.SetActive(false);
        }

        Debug.Log("[EpicIntroManager] Intro terminée.");

        yield return new WaitForSeconds(musicTransitionDelay);

        if (mainThemeMusic != null && musicSource != null)
        {
            float t = 0f;
            float initialVolume = musicSource.volume;
            while (t < musicFadeOutTime)
            {
                t += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(initialVolume, 0f, t / musicFadeOutTime);
                yield return null;
            }

            musicSource.clip = mainThemeMusic;
            musicSource.Play();

            t = 0f;
            while (t < musicFadeInTime)
            {
                t += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0f, 1f, t / musicFadeInTime);
                yield return null;
            }
        }
        if (introCamera != null)
            introCamera.gameObject.SetActive(false);

        OnIntroFinished?.Invoke();
    }

    private void Update()
    {
        if (allowSkip && Input.GetKeyDown(skipKey))
        {
            skipped = true;
            EndIntroInstantly();
        }
    }

    private void EndIntroInstantly()
    {
        StopAllCoroutines();

        if (fadeCanvas != null)
            fadeCanvas.alpha = 0f;

        foreach (var bubble in chatBubbles)
            if (bubble != null)
                bubble.SetActive(false);

        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = mainThemeMusic;
            musicSource.volume = 1f;
            musicSource.Play();
        }
        if (introCamera != null)
            introCamera.gameObject.SetActive(false);

        Debug.Log("[EpicIntroManager] Intro skipped manually.");
        OnIntroFinished?.Invoke();
    }
}
