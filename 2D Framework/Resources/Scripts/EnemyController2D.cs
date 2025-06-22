using UnityEngine;
using Fusion;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkTransform))]
public class EnemyController2D : NetworkBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    [Networked] private int currentHealth { get; set; }

    [Header("Combat")]
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    [Header("Detection")]
    public float attackRange = 1f;
    public float aggroRange = 5f;
    public LayerMask playerLayer;

    [Header("Animation & FX")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float knockbackForce = 5f;
    public float deathDelay = 1f;

    private Rigidbody2D rb;
    private Transform targetPlayer;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        HandleTargeting();
        HandleAttack();
        UpdateSortingOrder();
    }

    private void HandleTargeting()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, aggroRange, playerLayer);

        if (hit != null && hit.TryGetComponent(out PlayerNetworkController2D player))
        {
            targetPlayer = player.transform;

            // Orientation
            Vector2 dir = targetPlayer.position - transform.position;
            if (spriteRenderer != null)
                spriteRenderer.flipX = dir.x < 0;
            // Déplacement vers le joueur
            float distance = Vector2.Distance(transform.position, targetPlayer.position);
            if (distance > attackRange)
            {
                Vector2 direction = (targetPlayer.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * 2.5f * Runner.DeltaTime); // vitesse personnalisée
                animator?.SetBool("IsWalking", true);
            }
            else
            {
                animator?.SetBool("IsWalking", false);
            }

        }
        else
        {
            targetPlayer = null;
            animator?.SetBool("IsWalking", false);
        }
    }

    private void HandleAttack()
    {
        if (targetPlayer == null) return;

        float dist = Vector2.Distance(transform.position, targetPlayer.position);
        if (dist <= attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            if (targetPlayer.TryGetComponent(out PlayerNetworkController2D player))
            {
                player.TakeDamage(damage);
                animator?.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (animator)
            animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback();
        }
    }

    private void ApplyKnockback()
    {
        if (targetPlayer == null) return;

        Vector2 knockDir = (transform.position - targetPlayer.position).normalized;
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
    }

    private void Die()
    {
        Debug.Log($"[Enemy] {gameObject.name} est mort.");
        animator?.SetTrigger("Die");

        // Lancer la coroutine de délai avant despawn
        StartCoroutine(DelayedDespawn());
    }

    private IEnumerator DelayedDespawn()
    {
        yield return new WaitForSeconds(deathDelay);

        if (Object.HasStateAuthority)
            Runner.Despawn(Object);
    }


    private void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
