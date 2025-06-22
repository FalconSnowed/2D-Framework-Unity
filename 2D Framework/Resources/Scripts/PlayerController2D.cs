using UnityEngine;
using Fusion;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkTransform))]
public class PlayerNetworkController2D : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.05f;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public int maxHealth = 100;
    [Networked] private int currentHealth { get; set; }
    [Header("Attack")]
    public float attackCooldown = 0.8f;
    private float lastAttackTime = -999f;

    private Rigidbody2D rb;
    private Vector2 inputDirection;
    [Header("Sword Attack")]
    public float attackRange = 1.5f;
    public float attackDamage = 12f;
    public LayerMask enemyLayer;

    [Networked] private bool IsFlipped { get; set; }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        if (HasInputAuthority)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.gameObject.SetActive(true);
                cam.transform.position = new Vector3(0, 0, -10); // fixe
            }
        }
    }
    private IEnumerator FlashColor(Color flashColor, float duration)
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = original;
    }

    public void TakeDamage(int amount)
    {
        if (!Object.HasInputAuthority) return; // Optionnel : éviter double feedback

        currentHealth -= amount;

        Debug.Log($"[Player] Dégâts reçus: {amount}, HP restant: {currentHealth}");

        // Animation de hit
        if (animator != null)
            animator.SetTrigger("Hit");

        // Petit effet de flash rouge (exemple)
        if (spriteRenderer != null)
            StartCoroutine(FlashColor(Color.red, 0.1f));

        // Son optionnel
        // AudioSource.PlayClipAtPoint(hitSound, transform.position);

        if (currentHealth <= 0)
        {
            Debug.Log("[Player] 💀 Le joueur est mort.");

            if (animator != null)
                animator.SetTrigger("Death");

            // Désactivation temporaire des contrôles
            moveSpeed = 0f;

            // Optionnel : destruction réseau après X secondes
            Runner.Despawn(Object);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            HandleInput();
            HandleMovement();
            HandleAnimation();
            HandleFlip(); // met à jour IsFlipped localement
        }

        // ✅ Appliquer le flip à tous (y compris remote)
        spriteRenderer.flipX = IsFlipped;
    }

    private void HandleInput()
    {
        inputDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

       // if (Input.GetMouseButtonDown(0)) // 0 = clic gauche
           // TryAttack();

    }

    private void HandleMovement()
    {
        if (inputDirection != Vector2.zero)
        {
            bool moved = TryMove(inputDirection);

            if (!moved)
            {
                moved = TryMove(new Vector2(inputDirection.x, 0));
                if (!moved)
                    TryMove(new Vector2(0, inputDirection.y));
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        int hitCount = rb.Cast(direction, movementFilter, new RaycastHit2D[1], moveSpeed * Runner.DeltaTime + collisionOffset);

        if (hitCount == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Runner.DeltaTime);
            return true;
        }

        return false;
    }

    private void HandleAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", inputDirection.sqrMagnitude);
        }
    }

    private void HandleFlip()
    {
        if (spriteRenderer != null && inputDirection.x != 0f)
        {
            IsFlipped = inputDirection.x < 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
