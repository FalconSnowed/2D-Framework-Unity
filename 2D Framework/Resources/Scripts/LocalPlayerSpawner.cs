using UnityEngine;
using Fusion;
using System.Collections;

public class LocalPlayerSpawner : MonoBehaviour
{
    [Header("Player Prefab & Spawn")]
    public NetworkPrefabRef playerPrefab;
    public Transform spawnPoint;
    public float retryInterval = 1.5f;
    public int maxRetries = 5;

    private NetworkRunner runner;
    private Coroutine spawnCoroutine;

    [System.Obsolete]
    void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();

        if (runner != null && runner.IsRunning)
        {
            spawnCoroutine = StartCoroutine(TrySpawnPlayer());
        }
        else
        {
            Debug.LogWarning("[LocalPlayerSpawner] Runner not found or not running.");
        }
    }

    private IEnumerator TrySpawnPlayer()
    {
        int attempt = 0;

        while (attempt < maxRetries)
        {
            if (playerPrefab.IsValid && spawnPoint != null && runner != null && runner.LocalPlayer != null)
            {
                NetworkObject spawned = runner.Spawn(playerPrefab, spawnPoint.position, Quaternion.identity, runner.LocalPlayer);
                if (spawned != null)
                {
                    Debug.Log($"[LocalPlayerSpawner] Player spawned after {attempt + 1} attempt(s).");
                    yield break;
                }
                else
                {
                    Debug.LogWarning($"[LocalPlayerSpawner] Spawn failed. Retrying...");
                }
            }
            else
            {
                Debug.LogWarning($"[LocalPlayerSpawner] Conditions not met. Retrying...");
            }

            attempt++;
            yield return new WaitForSeconds(retryInterval);
        }

        Debug.LogError("[LocalPlayerSpawner] Spawn failed after max retries.");
    }
}
