using System.Collections;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class SwordAttack : NetworkBehaviour
{
    public Collider2D swordCollider;
    public float damage = 3f;
    public float attackCooldown = 1.0f;
    public AudioClip attackSound; // 🔊 À glisser dans l’Inspector
    public AudioSource audioSource;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private Vector2 attackOffset;
    Animator animator;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (swordCollider != null)
            swordCollider.enabled = false;
    }


    public override void Spawned()
    {
        animator = GetComponent<Animator>();
        if (swordCollider != null)
        {
            attackOffset = swordCollider.transform.localPosition;
            swordCollider.isTrigger = true;
            swordCollider.enabled = false;
        }
    }

    public void Attack(bool isFacingRight)
    {
        if (Time.time - lastAttackTime < attackCooldown || isAttacking || swordCollider == null) return;

        isAttacking = true;

        // Positionner le collider à gauche ou droite
        Vector3 offset = isFacingRight ? attackOffset : new Vector3(-attackOffset.x, attackOffset.y);
        swordCollider.transform.localPosition = offset;
        animator.SetTrigger("swordAttack");

        // 🎵 Son d’attaque
        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        // Activer le collider brièvement
        swordCollider.enabled = true;
        StartCoroutine(DisableColliderAfterDelay(0.2f));

        lastAttackTime = Time.time;
    }

    private IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (swordCollider != null)
            swordCollider.enabled = false;

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Object || !Object.HasStateAuthority) return;

        if (isAttacking)
        {
            if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            else if (other.CompareTag("Player") && other.TryGetComponent(out PlayerNetworkController2D player))
            {
                player.TakeDamage((int)damage);
            }
        }
    }

    private void OnDisable()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;
    }
}
