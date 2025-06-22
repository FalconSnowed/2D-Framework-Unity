using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

[RequireComponent(typeof(NetworkTransform), typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour
{
    public bool isFacingRight = true;
    public Camera playerCamera;
    private AudioListener audioListener;
    private PlayerSpells spells;
    [SerializeField] private TMP_Text usernameText;
    public string playerName = "Hero";
    public string className = "Guerrier";

    [Header("Refs")]
    private float lastHitTime = -999f;
    public float hitCooldown = 1.0f;

    public int attack = 12;
    public int defense = 5;
    public int maxHealth = 100;
    [Networked] public int currentHealth { get; set; }

    public Texture2D portrait;

    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    public float enduranceRegenRate = 5f;
    public float dashSpeedMultiplier = 2f;
    public float maxMana = 100f;
    [Networked] public float currentMana { get; set; }
    public float dashEnduranceCost = 20f;
    public float dashDistance = 5f;
    public float maxEndurance = 100f;
    [Networked] public float currentEndurance { get; set; }
    public float dashDuration = 0.5f;
    private bool isDashing = false;

    public Slider healthSlider;
    public Slider manaSlider;
    public Slider enduranceSlider;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    readonly List<RaycastHit2D> castCollisions = new();

    public bool canMove = true;
    public new Camera camera;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spells = GetComponent<PlayerSpells>();
        audioListener = GetComponent<AudioListener>();

        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEndurance = maxEndurance;

        if (Object.HasInputAuthority)
        {
            playerCamera.gameObject.SetActive(true);
        }
        if (HasInputAuthority)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.gameObject.SetActive(true);
                cam.transform.position = new Vector3(0, 0, -10); // fixe
            }
        }
        UpdateHealthBar();
        UpdateManaBar();
        UpdateEnduranceBar();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority || !canMove) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        movementInput = input;

        if (movementInput != Vector2.zero && !isDashing)
            TryMove(movementInput);

        animator.SetBool("isMoving", movementInput != Vector2.zero);
        spriteRenderer.flipX = movementInput.x < 0;

        if (Input.GetKeyDown(KeyCode.Space) && currentEndurance >= dashEnduranceCost)
        {
           // Runner.StartCoroutine(DashFunction());
        }

        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            
            SwordAttack();
        }

        RegenerateEndurance();
    }

    IEnumerator DashFunction()
    {
        UseEndurance(dashEnduranceCost);
        isDashing = true;

        Vector2 initialPosition = rb.position;
        Vector2 dashDestination = initialPosition + movementInput.normalized * dashDistance;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(initialPosition, dashDestination, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;
        Vector2 newPosition = rb.position + moveSpeed * Runner.DeltaTime * direction;
        int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Runner.DeltaTime + collisionOffset);
        if (count == 0)
        {
            rb.MovePosition(newPosition);
            return true;
        }
        return false;
    }

    void RegenerateEndurance()
    {
        if (!isDashing)
        {
            currentEndurance += enduranceRegenRate * Runner.DeltaTime;
            currentEndurance = Mathf.Min(currentEndurance, maxEndurance);
            UpdateEnduranceBar();
        }
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        int mitigated = Mathf.Max(damage - defense, 1);
        currentHealth -= mitigated;
        currentHealth = Mathf.Max(currentHealth, 0);

        animator.SetTrigger("Hit");
        Runner.StartCoroutine(FlashRed());
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            canMove = false;
            Runner.Despawn(Object);
        }
    }

    IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    public void UpdateHealthBar()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void UpdateManaBar()
    {
        manaSlider.value = currentMana / maxMana;
    }

    public void UpdateEnduranceBar()
    {
        enduranceSlider.value = currentEndurance / maxEndurance;
    }

    void UseEndurance(float cost)
    {
        currentEndurance -= cost;
        currentEndurance = Mathf.Max(currentEndurance, 0);
        UpdateEnduranceBar();
    }

    void SwordAttack()
    {
        if (swordAttack != null)
        {
            swordAttack.Attack(!spriteRenderer.flipX); // !flip = droite
        }
    }

}
