using Fusion;
using UnityEngine;

public class MysticCharacter : NetworkBehaviour
{
    [Networked] public Vector3 Position { get; set; } // Position synchronisée
    [Networked] public int Health { get; set; } // Santé synchronisée
    [Networked] public bool IsNearSage { get; set; } // État narratif synchronisé

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 100;

    private CharacterController controller;

    // Appelée une fois au spawn
    public override void Spawned()
    {
        if (Object.HasStateAuthority) // Seulement l'autorité (host ou server) initialise
        {
            Health = maxHealth;
            Position = transform.position;
        }
        controller = GetComponent<CharacterController>();
    }

    // Mise à jour par tick (synchronisée)
    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority) // Seulement le joueur local bouge
        {
            MoveCharacter();
            CheckSageProximity();
        }

        // Applique la position synchronisée
        transform.position = Position;
    }

    // Gestion du mouvement (local)
    private void MoveCharacter()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 move = moveDirection * moveSpeed * Runner.DeltaTime;
            controller.Move(move);
            Position += move; // Met à jour la position pour synchronisation
        }
    }

    // Détection de proximité avec le Sage (trigger narratif)
    private void CheckSageProximity()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); // Rayon de 2 unités
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Sage"))
            {
                if (!IsNearSage)
                {
                    IsNearSage = true;
                    if (Object.HasStateAuthority)
                    {
                        Debug.Log("Le Sage est proche ! Une quête commence...");
                        // Ajoute ici une logique narrative (dialogue, quête, etc.)
                    }
                }
                return;
            }
        }
        IsNearSage = false;
    }

    // RPC pour une action multijoueur (ex. : tous reçoivent un message)
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AnnounceSageEncounter(string message)
    {
        Debug.Log($"[Multijoueur] {message}");
        // Tu peux ajouter une UI ou un effet ici
    }

    // Exemple d'appel quand le Sage est détecté
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sage") && Object.HasStateAuthority)
        {
            RPC_AnnounceSageEncounter("Le Sage révèle un secret ancien...");
        }
    }
}