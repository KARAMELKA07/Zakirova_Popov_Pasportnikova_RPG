using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnPoint;

    void Start()
    {
        if (bossPrefab != null && spawnPoint != null)
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("BossSpawner: назначь bossPrefab и spawnPoint!");
        }

        gameObject.SetActive(false); // отключаем спавнер сразу после
    }
}