using Fusion;
using UnityEngine;

public class PlayerSpawnerNetworked : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform spawnPoint; // ⬅️ Référence manuelle dans l’inspecteur

    public void PlayerJoined(PlayerRef player)
    {
        if (!playerPrefab.IsValid || Runner == null)
        {
            Debug.LogWarning("[PlayerSpawner] Prefab or Runner not set.");
           // return;
        }

        if (Runner.TryGetPlayerObject(player, out _))
        {
            Debug.Log($"[PlayerSpawner] Player {player.PlayerId} already spawned.");
          //  return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        NetworkObject playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        if (playerObject != null)
            Debug.Log($"[PlayerSpawner] Spawned player {player.PlayerId} at {spawnPosition}");
        else
            Debug.LogError($"[PlayerSpawner] Failed to spawn player {player.PlayerId}");
    }
}
