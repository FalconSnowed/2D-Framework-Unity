using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Fusion;

public class EnemyDeathEvent : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string deathTriggerName = "Death";

    [Header("Teleport")]
    public Transform teleportTarget;
    public float delayBeforeTeleport = 1.5f;

    [Header("VFX & SFX")]
    public GameObject deathVFXPrefab;
    public AudioClip deathSound;
    public AudioSource audioSource;

    [Header("Text")]
    public Text uiText;
    public string messageToDisplay = "The enemy collapses and vanishes...";
    public float textDuration = 3f;

    [Header("Auto Cleanup")]
    public bool disableAfter = true;

    private bool isDead = false;

    [System.Obsolete]
    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(DeathSequence());
    }

    [System.Obsolete]
    private IEnumerator DeathSequence()
    {
        // 1. Animation
        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
            animator.SetTrigger(deathTriggerName);

        // 2. VFX
        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        // 3. SFX
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // 4. Texte
        if (uiText != null)
        {
            uiText.text = messageToDisplay;
            yield return new WaitForSeconds(textDuration);
            uiText.text = "";
        }
        else
        {
            Debug.Log(messageToDisplay);
        }
        // 🔁 Téléporte le joueur local via RPC
        yield return new WaitForSeconds(delayBeforeTeleport);

        foreach (var playerObj in FindObjectsOfType<NetworkObject>())
        {
            if (playerObj.TryGetComponent(out PlayerController controller))
            {
                if (playerObj.HasStateAuthority) // 👑 celui qui peut faire le Rpc
                {
                    controller.Rpc_TeleportTo(teleportTarget.position);
                    break;
                }
            }
        }
        // 5. Attendre avant TP
        yield return new WaitForSeconds(delayBeforeTeleport);

        // 6. TP du joueur local
        if (teleportTarget != null)
        {
            // Recherche du joueur local
            var players = FindObjectsOfType<NetworkObject>();
            foreach (var p in players)
            {
                if (p.HasInputAuthority)
                {
                    p.transform.position = teleportTarget.position;

                    // Centrage de la caméra si fixe
                    if (Camera.main != null)
                        Camera.main.transform.position = teleportTarget.position + new Vector3(0, 0, -10);

                    Debug.Log($"[EnemyDeathEvent] Local player teleported to {teleportTarget.position}");
                    break;
                }
            }
        }

        if (disableAfter)
            gameObject.SetActive(false);
    }
}
