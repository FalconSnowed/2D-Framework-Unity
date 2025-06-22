using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CrystalTeleportAndDamage : MonoBehaviour
{
    [Header("Téléportation")]
    public Transform teleportTarget;
    public Transform cameraPointAfterTeleport;
    public AudioClip tpSound;
    public string cinematicMessage = "✨ Le cristal vous transporte ailleurs...";

    [Header("Zone instable autour du cristal (après TP)")]
    public float dangerRadius = 6f;
    public float damageInterval = 1.5f;
    public int damageAmount = 8;

    [Header("Blocage avant TP")]
    public GameObject obstacleToDisable; // Ex: mur invisible
    public string blockedMessage = "Mieux vaut pas que j'aille par là-bas...";
    public float messageDuration = 3f;

    private bool playerTeleported = false;
    private float damageTimer = 0f;
    private GameObject player;
    private PlayerController controller;

    // GUI
    private string currentMessage = "";
    private float messageTimer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.gameObject;
        controller = player.GetComponent<PlayerController>();

        if (playerTeleported)
            return;

        if (controller != null && teleportTarget != null)
        {
            controller.transform.position = teleportTarget.position;
            playerTeleported = true;
            damageTimer = 0f;

            if (CameraFixedZoneController.Instance != null && cameraPointAfterTeleport != null)
                CameraFixedZoneController.Instance.MoveCameraTo(cameraPointAfterTeleport);

            if (tpSound != null)
                AudioSource.PlayClipAtPoint(tpSound, teleportTarget.position);

            if (obstacleToDisable != null)
                obstacleToDisable.SetActive(false);

            ShowMessage(cinematicMessage);
        }
    }

    private void Update()
    {
        if (!playerTeleported || player == null || controller == null)
            return;

        float distance = Vector2.Distance(player.transform.position, teleportTarget.position);

        if (distance < dangerRadius)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                controller.TakeDamage(damageAmount);
                ShowMessage("⚠️ Zone instable ! Vous subissez des dégâts !");
            }
        }
        else
        {
            damageTimer = 0f;
        }

        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
                currentMessage = "";
        }
    }

    /// <summary>
    /// Peut être appelé par un autre script ou trigger si le joueur tente d’entrer sans le cristal.
    /// </summary>
    public void WarnPlayer()
    {
        if (!playerTeleported)
        {
            ShowMessage(blockedMessage);
        }
    }

    private void ShowMessage(string message)
    {
        currentMessage = message;
        messageTimer = messageDuration;
    }

    private void OnGUI()
    {
        if (!string.IsNullOrEmpty(currentMessage))
        {
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                wordWrap = true
            };

            float width = 500f;
            float height = 80f;
            float x = (Screen.width - width) / 2f;
            float y = Screen.height * 0.8f;

            GUI.Box(new Rect(x, y, width, height), currentMessage, style);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (teleportTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(teleportTarget.position, dangerRadius);
        }
    }
}
