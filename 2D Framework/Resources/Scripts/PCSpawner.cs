using UnityEngine;
using Fusion;
using System.Collections;

public class PCSpawner : MonoBehaviour
{
    [Header("NPC Settings")]
    public NetworkPrefabRef npcPrefab;
    public Transform[] spawnPoints;
    private NetworkRunner runner;
    private NetworkObject spawnedNPC;

    private void Start()
    {
        StartCoroutine(InitializeSpawner());
    }

    private IEnumerator InitializeSpawner()
    {
        while (runner == null)
        {
            runner = FindObjectOfType<NetworkRunner>();
            if (runner == null)
            {
                Debug.LogWarning("⚠ En attente du NetworkRunner...");
                yield return new WaitForSeconds(1f);
            }
        }

        Debug.Log("✅ NetworkRunner trouvé. En attente de l'initialisation...");

        while (!runner.IsRunning)
        {
            Debug.Log("⚠ En attente que le NetworkRunner soit en cours d'exécution...");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("✅ NetworkRunner prêt. Mode : " + runner.GameMode);

        // Seul le serveur ou le SharedModeMasterClient peut spawner
        if (runner.IsServer || runner.IsSharedModeMasterClient)
        {
            Debug.Log("✅ Autorité de spawn détectée. Vérification de l'existence du NPC...");
            VerifyAndSpawnNPC();
        }
        else
        {
            Debug.LogError("⚠ Vous n'avez pas l'autorité pour spawner les NPC.");
        }
    }

    private void VerifyAndSpawnNPC()
    {
        // Rechercher tous les objets réseau et vérifier si un NPC existe déjà
        NetworkObject[] allNetworkObjects = FindObjectsOfType<NetworkObject>();
        foreach (var obj in allNetworkObjects)
        {
            if (obj.name.Contains("Fusion_NPC_Spawned"))
            {
                Debug.Log("✅ NPC déjà existant dans la scène. Aucun nouveau spawn.");
                spawnedNPC = obj;
                return;
            }
        }

        // Si aucun NPC trouvé, on spawn un nouveau
        Debug.Log("✅ Aucun NPC existant. Spawning...");
        SpawnNPC();
    }

    private void SpawnNPC()
    {
        if (runner == null)
        {
            Debug.LogError("⚠ NetworkRunner est null. Impossible de spawner.");
            return;
        }

        if (npcPrefab == null)
        {
            Debug.LogError("⚠ NPC Prefab non assigné !");
            return;
        }

        if (spawnedNPC != null)
        {
            Debug.Log("✅ Un NPC existe déjà, aucun nouveau spawn.");
            return;
        }

        // Sélectionner un point de spawn aléatoire
        Vector3 spawnPosition = spawnPoints.Length > 0
            ? spawnPoints[Random.Range(0, spawnPoints.Length)].position
            : new Vector3(0f, 0f, 0);

        // Spawn sécurisé avec un nom unique
        spawnedNPC = runner.Spawn(npcPrefab, spawnPosition, Quaternion.identity);
        if (spawnedNPC != null)
        {
            spawnedNPC.name = $"Fusion_NPC_Spawned_{spawnedNPC.Id}";
            Debug.Log($"✅ NPC Spawned dynamiquement par le réseau à la position {spawnPosition}.");
        }
        else
        {
            Debug.LogError("⚠ Echec du spawn du NPC.");
        }
    }
}
