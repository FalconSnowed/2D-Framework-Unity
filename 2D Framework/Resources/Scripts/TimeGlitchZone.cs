using UnityEngine;
using Fusion;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TimeGlitchZone : NetworkBehaviour
{
    public float glitchDuration = 4f;
    public float timeScaleFactor = 0.3f;
    public AudioClip glitchSound;
    public ParticleSystem glitchVFX;

    public Material glitchMaterial;
    public SpriteRenderer affectedRenderer;

    private AudioSource audioSource;
    private Material originalMaterial;
    private bool glitching = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (affectedRenderer != null)
            originalMaterial = affectedRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!HasStateAuthority || glitching) return;

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            StartCoroutine(GlitchRoutine(player));
        }
    }

    private IEnumerator GlitchRoutine(PlayerController player)
    {
        glitching = true;

        // 🌀 Activer VFX et SFX
        glitchVFX?.Play();
      //  if (glitchSound != null)
          //  audioSource?.PlayOneShot(glitchSound);

        // 🎞️ Effet shader
        if (affectedRenderer != null && glitchMaterial != null)
            affectedRenderer.material = glitchMaterial;

        // ⏳ Modifier l'écoulement du temps local (genre bug)
        float originalSpeed = player.moveSpeed;
        player.moveSpeed *= timeScaleFactor;

        // 🌫️ Eventuellement désactive certains inputs
        player.canMove = false;
        yield return new WaitForSeconds(glitchDuration / 2f);
        player.canMove = true;

        // 🪐 TP ou désynchronisation facultative
        if (Random.value < 0.5f)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            player.Rpc_TeleportTo(player.transform.position + randomOffset);
        }

        yield return new WaitForSeconds(glitchDuration / 2f);

        // 🔁 Reset
        player.moveSpeed = originalSpeed;
        if (affectedRenderer != null && originalMaterial != null)
            affectedRenderer.material = originalMaterial;

        glitching = false;
    }
}
