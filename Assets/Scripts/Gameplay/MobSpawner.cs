using UnityEngine;
using System.Collections.Generic;

public class MobSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxMeleeEnemies = 3;
    [SerializeField] private int maxRangedEnemies = 2;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);

    [Header("Collision Check")]
    [SerializeField] private float checkRadius = 1.0f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private int maxAttempts = 10;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        // Спавн ближних врагов
        for (int i = 0; i < maxMeleeEnemies; i++)
        {
            if (TryGetValidSpawnPosition(out Vector3 spawnPos))
            {
                SpawnMeleeEnemy(spawnPos);
            }
        }

        // Спавн дальних врагов
        for (int i = 0; i < maxRangedEnemies; i++)
        {
            if (TryGetValidSpawnPosition(out Vector3 spawnPos))
            {
                Instantiate(rangedEnemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    bool TryGetValidSpawnPosition(out Vector3 result)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 candidate = GetRandomSpawnPosition();

            if (!Physics.CheckSphere(candidate, checkRadius, collisionMask))
            {
                result = candidate;
                return true;
            }
        }

        Debug.LogWarning("Failed to find valid spawn position");
        result = Vector3.zero;
        return false;
    }

    void SpawnMeleeEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(meleeEnemyPrefab, position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }

    Vector3 GetRandomSpawnPosition()
    {
        return transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}