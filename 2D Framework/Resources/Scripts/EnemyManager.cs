using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // Instance statique de l'EnemyManager

    public GameObject enemyPrefabType1;
    public GameObject enemyPrefabType2;
    public Transform[] enemySpawnPoints;

    private void Awake()
    {
        // S'assurer qu'il n'y a qu'une seule instance de EnemyManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Si une instance existe déjà, détruisez celle-ci
           // Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Appel pour commencer à générer les ennemis au début du jeu
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            Transform enemySpawnPoint = enemySpawnPoints[i];

            // Alternez entre les ennemis de type 1 et de type 2 à chaque itération
            if (i % 2 == 0)
            {
                Instantiate(enemyPrefabType1, enemySpawnPoint.position, Quaternion.identity);
            }
            else
            {
                Instantiate(enemyPrefabType2, enemySpawnPoint.position, Quaternion.identity);
            }
        }
    }
}
