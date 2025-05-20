using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public static MobManager Instance { get; private set; }

    private List<GameObject> activeMobs = new List<GameObject>();
    private int killCount = 0;
    private bool bossSpawned = false;
    private bool victoryMusicPlayed = false;

    public GameObject bossSpawner;
    public int killsToSpawnBoss = 3;
    public int killsToVictoryMusic = 5;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RegisterMob(GameObject mob)
    {
        activeMobs.Add(mob);
    }

    public void UnregisterMob(GameObject mob)
    {
        activeMobs.Remove(mob);
        Destroy(mob, 3f);

        killCount++;
        Debug.Log($"Убито мобов: {killCount}");

        if (killCount == killsToSpawnBoss && !bossSpawned)
        {
            SpawnBoss();
        }

        if (killCount == killsToVictoryMusic && !victoryMusicPlayed)
        {
            PlayVictoryMusic();
        }
        ScoreUI.Instance?.UpdateScore(killCount);

    }

    public int GetMobCount() => activeMobs.Count;

    private void SpawnBoss()
    {
        if (bossSpawner != null)
        {
            bossSpawner.SetActive(true);
            bossSpawned = true;
            Debug.Log("Босс заспавнен!");
        }
        else
        {
            Debug.LogWarning("BossSpawner не назначен в MobManager!");
        }
    }

    private void PlayVictoryMusic()
    {
        GameBootstrapper.Instance.AudioService.PlayMusic("victory");
        victoryMusicPlayed = true;
        Debug.Log("Победная музыка играет!");
    }
}