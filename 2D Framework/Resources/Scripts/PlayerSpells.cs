using UnityEngine;

public class PlayerSpells : MonoBehaviour
{
    public bool aoeUnlocked = false;
    public GameObject aoeSpellEffectPrefab;
    public float aoeRadius = 2.5f;
    public float aoeDamage = 3f;

    public LayerMask enemyLayer; // à assigner dans l'inspecteur : couche Enemy

    public void UnlockAOESpell()
    {
        if (!aoeUnlocked)
        {
            aoeUnlocked = true;
            Debug.Log("✨ Sort AOE débloqué !");
        }
    }
    private float aoeCooldown = 2.5f;
    private float lastCastTime = -10f;

    public bool IsAOEOnCooldown()
    {
        return Time.time - lastCastTime < aoeCooldown;
    }

    public float RemainingCooldown()
    {
        return Mathf.Max(0f, aoeCooldown - (Time.time - lastCastTime));
    }

    public void CastAOE()
    {
        if (!aoeUnlocked || IsAOEOnCooldown()) return;

        // Effet visuel
        if (aoeSpellEffectPrefab != null)
            Instantiate(aoeSpellEffectPrefab, transform.position, Quaternion.identity);
        lastCastTime = Time.time;
        Debug.Log("💥 Sort AOE lancé !");
        // Dégâts
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius, enemyLayer);
        foreach (Collider2D col in hitEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(aoeDamage);
            }
        }

        Debug.Log($"💥 Sort AOE infligé à {hitEnemies.Length} ennemis !");
    }

    private void OnDrawGizmosSelected()
    {
        // Pour visualiser le rayon AOE dans l’éditeur Unity
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
