using UnityEngine;
using Fusion;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSortingByY : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Networked] private bool IsFlipped { get; set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void FixedUpdateNetwork()
    {
        // 🔁 Si c'est l'autorité locale, mettre à jour la valeur réseau
        if (HasInputAuthority)
        {
            IsFlipped = spriteRenderer.flipX;
        }

        // 🔄 Appliquer à tous la bonne orientation
        spriteRenderer.flipX = IsFlipped;
    }

    void LateUpdate()
    {
        // 🧠 Système de tri selon Y pour isométrique
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
