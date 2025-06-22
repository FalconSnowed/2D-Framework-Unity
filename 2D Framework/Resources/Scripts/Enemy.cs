using UnityEngine;
using Fusion;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public float sightRange = 10f;
    public float separationRadius = 1.5f;
    public Slider healthSlider;
    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioSource audioSource;
    [Header("Attack Collider")]
    public Collider2D attackCollider;
    public float attackEnableTime = 0.2f;

    [Header("Combat")]
    public float baseDamage = 8f;
    public float critChance = 0.15f;
    public float critMultiplier = 1.75f;

    public enum BehaviorType { Charger, Orbit }
    public BehaviorType behaviorType = BehaviorType.Charger;
    private float orbitAngle;
    public float orbitSpeed = 45f;

    [Networked] public float Health { get; set; }
    [Networked] public NetworkBool IsDead { get; set; }

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isAttacking = false;
    private Vector2 moveDirection;
    private float attackTimer = 0f;
    private float idleCooldown = 0f;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
            attackCollider.isTrigger = true;
        }

        if (HasStateAuthority)
        {
            Health = 50f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsDead || !HasStateAuthority) return;

        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null) player = target.transform;
        }

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < sightRange && !IsIdle())
        {
            Vector2 direction = behaviorType == BehaviorType.Orbit ? GetOrbitDirection() : (player.position - transform.position).normalized;
            Vector2 separation = ComputeSeparation();
            Vector2 desiredDirection = (direction + separation).normalized;
            float stopDistance = attackRange * 0.9f;

            if (distance > stopDistance)
                moveDirection = desiredDirection;
            else
                moveDirection = Vector2.zero;


            if (distance <= attackRange && attackTimer <= 0f)
            {
                StartCoroutine(PerformAttack());
            }
        }
        else
        {
            moveDirection = Vector2.zero;
        }

        if (attackTimer > 0f)
            attackTimer -= Runner.DeltaTime;

        if (idleCooldown > 0f)
            idleCooldown -= Runner.DeltaTime;
        else if (Random.value < 0.005f)
            idleCooldown = Random.Range(1f, 2f);

        // 🔁 Mise à jour de "Speed" dans l'Animator
        float currentSpeed = moveDirection.magnitude * moveSpeed;
        animator?.SetFloat("Speed", currentSpeed);

        if (moveDirection != Vector2.zero && !IsIdle())
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Runner.DeltaTime);
            if (moveDirection.x != 0)
                spriteRenderer.flipX = moveDirection.x < 0;
        }

        UpdateHealthBar();
    }

    private bool IsIdle() => idleCooldown > 0f;

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        animator.SetTrigger("Attack");

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        // ✅ Activer le collider d'attaque temporairement
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            yield return new WaitForSeconds(attackEnableTime);
            attackCollider.enabled = false;
        }

        yield return new WaitForSeconds(attackCooldown - attackEnableTime);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Object.HasStateAuthority || !isAttacking) return;

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            float damage = baseDamage * (Random.value < critChance ? critMultiplier : 1f);
            player.TakeDamage((int)damage);
        }
    }

    private Vector2 ComputeSeparation()
    {
        Vector2 force = Vector2.zero;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var col in colliders)
        {
            if (col != null && col.gameObject != gameObject && col.CompareTag("Enemy"))
            {
                Vector2 away = (Vector2)(transform.position - col.transform.position);
                force += away.normalized / Mathf.Max(away.magnitude, 0.01f);
            }
        }

        return force * 0.5f;
    }

    private Vector2 GetOrbitDirection()
    {
        orbitAngle += orbitSpeed * Runner.DeltaTime;
        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * (attackRange + 0.5f);
        Vector2 targetPos = (Vector2)player.position + offset;
        return (targetPos - (Vector2)transform.position).normalized;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        Health -= amount;
        if (Health <= 0)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        moveDirection = Vector2.zero;

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        animator.SetTrigger("Defeated");

        StartCoroutine(DelayedDespawn(1.56f)); // 🕒 Laisse 3 secondes pour l’anim
    }

    private IEnumerator DelayedDespawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Object != null && Object.IsValid)
            Runner.Despawn(Object);
    }


    private void UpdateHealthBar()
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.Clamp01(Health / 50f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
