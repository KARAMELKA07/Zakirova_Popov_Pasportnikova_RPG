using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [SerializeField] private Transform[] meleeSpawnPoints; 
    [SerializeField] private Transform[] rangedSpawnPoints; 

    [SerializeField] private int maxMeleeEnemies = 3;
    [SerializeField] private int maxRangedEnemies = 2;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < maxMeleeEnemies; i++)
        {
            Transform spawnPoint = meleeSpawnPoints[i % meleeSpawnPoints.Length]; // Берем по очереди точки
            SpawnEnemy(meleeEnemyPrefab, spawnPoint);
        }

        for (int i = 0; i < maxRangedEnemies; i++)
        {
            Transform spawnPoint = rangedSpawnPoints[i % rangedSpawnPoints.Length];
            SpawnEnemy(rangedEnemyPrefab, spawnPoint);
        }
    }

    void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
        MobManager.Instance.RegisterMob(enemy);
    }
}
