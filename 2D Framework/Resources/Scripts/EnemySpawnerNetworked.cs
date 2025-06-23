using UnityEngine;
using Fusion;

public class EnemySpawnerNetworked : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public float spawnInterval = 10f;
    private float spawnTimer;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        spawnTimer = spawnInterval;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || enemyPrefab.Equals(default)) return;

        spawnTimer -= Runner.DeltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 position = point != null ? point.position : Vector3.zero;

        NetworkObject enemy = Runner.Spawn(enemyPrefab, position, Quaternion.identity);

        if (enemy != null)
        {
            Debug.Log($"[EnemySpawner] Spawned enemy at {position}");
        }
    }
}
