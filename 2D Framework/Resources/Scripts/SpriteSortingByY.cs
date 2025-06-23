using UnityEngine;
using Fusion;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSortingByY : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Networked] private bool IsFlipped { get; set; }

    public override void Spawned()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialisation de l'état réseau à l'apparition (pour éviter les défauts au spawn)
        if (HasStateAuthority)
        {
            IsFlipped = spriteRenderer.flipX;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // ✅ MAJ de l'état flipX uniquement par celui qui contrôle l'objet
        if (HasInputAuthority || HasStateAuthority)
        {
            IsFlipped = spriteRenderer.flipX;
        }

        // ✅ Appliquer la direction synchronisée sur tous les clients
        spriteRenderer.flipX = IsFlipped;
    }

    void LateUpdate()
    {
        // 🧠 Tri Y dynamique pour style isométrique
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}
