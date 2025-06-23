using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawnerNetworked : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform spawnPoint;

    private HashSet<PlayerRef> spawnedPlayers = new();
    private bool alreadySpawned = false;

    public void PlayerJoined(PlayerRef player)
    {
        if (alreadySpawned)
        {
            Debug.LogWarning("🚫 Player spawn already executed.");
            return;
        }

        if (!Runner || !playerPrefab.IsValid)
            return;

        if (Runner.TryGetPlayerObject(player, out var obj) && obj != null)
        {
            Debug.Log($"Player {player.PlayerId} already spawned.");
            return;
        }

        // ✨ Décale les spawns pour éviter empilement
        Vector3 spawnOffset = GetSpawnOffset(player);
        Vector3 spawnPosition = spawnPoint ? spawnPoint.position + spawnOffset : spawnOffset;

        var playerObj = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
        if (playerObj != null)
        {
            alreadySpawned = true;
            Debug.Log($"✅ Spawned player {player.PlayerId} at {spawnPosition}");
        }
    }
    private Vector3 GetSpawnOffset(PlayerRef player)
    {
        float radius = 2f;
        int id = player.PlayerId;
        float angle = id * 137.5f * Mathf.Deg2Rad; // Phyllotaxis offset
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }

}
