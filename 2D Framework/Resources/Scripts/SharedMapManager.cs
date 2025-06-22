using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using static Unity.Collections.Unicode;

public class SharedMapManager : NetworkBehaviour, ISceneLoadDone
{
    [Header("Network Prefabs")]
    public NetworkPrefabRef playerPrefab;
    public Transform spawnPoint;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip zoneMusic;
    public AudioClip ambientLoop;

    [Header("Cinematic")]
    public CanvasGroup introFade;
    public float fadeDuration = 2f;
    public Camera introCamera;

    [Header("Visual Layers")]
    public List<GameObject> foregroundLayers;
    public List<GameObject> backgroundLayers;
    public Animator environmentAnimator;

    [Header("Embellishments")]
    public List<ParticleSystem> particleFX;
    public Light directionalLight;
    public Color zoneLightColor;

    private GameObject localPlayerInstance;

    public override void Spawned()
    {
        if (Runner.GameMode != GameMode.Shared || !Object.HasStateAuthority)

            return;

        StartCoroutine(InitializeScene());
    }

    private IEnumerator InitializeScene()
    {
        // Fade-in
        if (introFade != null)
        {
            introFade.alpha = 1f;
            float t = 0f;
            while (t < fadeDuration)
            {
                introFade.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            introFade.alpha = 0f;
        }

        // Désactivation de la caméra d’intro
        if (introCamera != null)
            introCamera.gameObject.SetActive(false);

        // Spawn du joueur
        if (playerPrefab.IsValid && Runner.LocalPlayer != null && spawnPoint != null)
        {
            NetworkObject obj = Runner.Spawn(playerPrefab, spawnPoint.position, Quaternion.identity, Runner.LocalPlayer);
            localPlayerInstance = obj.gameObject;
        }

        // Audio
        if (musicSource != null)
        {
            musicSource.clip = zoneMusic;
            musicSource.loop = true;
            musicSource.Play();
            if (ambientLoop != null)
                musicSource.PlayOneShot(ambientLoop);
        }

        // Lumière
        if (directionalLight != null)
            directionalLight.color = zoneLightColor;

        // FX
        foreach (var fx in particleFX)
        {
            if (fx != null) fx.Play();
        }

        // Animation
        if (environmentAnimator != null)
            environmentAnimator.SetTrigger("PlayZone");

        // Layers
        SetLayersActive(foregroundLayers, true);
        SetLayersActive(backgroundLayers, true);
    }

    private void SetLayersActive(List<GameObject> layers, bool state)
    {
        foreach (var layer in layers)
        {
            if (layer != null)
                layer.SetActive(state);
        }
    }

    public void SceneLoadDone(in SceneLoadDoneArgs args)
    {
        Debug.Log("[SharedMapManager] Scene fully loaded and initialized.");
    }
}
