using System;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public GameObject[] lootItems; // Objets de loot possibles
    public float[] dropChances; // Chances de drop pour chaque item

    public void DropLoot()
    {
        for (int i = 0; i < lootItems.Length; i++)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f);
            if (randomValue < dropChances[i])
            {
                Instantiate(lootItems[i], transform.position, Quaternion.identity);
            }
        }
    }

    public void OnEnemyDefeated() // Appelé à la mort de l'ennemi
    {
        DropLoot();
    }
}
