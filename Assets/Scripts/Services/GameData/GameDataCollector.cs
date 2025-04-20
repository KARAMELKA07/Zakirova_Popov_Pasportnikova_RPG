using UnityEngine;

public static class GameDataCollector
{
    public static GameData Collect()
    {
        GameData data = new GameData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position;

            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                data.playerHP = health.GetCurrentHP();
            }
        }

        // о да, лутаем доп баллы
        foreach (var mob in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var mobHealth = mob.GetComponent<MobHealth>();
            if (mobHealth != null)
            {
                data.mobs.Add(new MobSaveData
                {
                    mobType = mob.name.Replace("(Clone)", "").Trim(), // имя префаба
                    position = mob.transform.position,
                    hp = mobHealth.GetCurrentHP()
                });
            }
        }

        return data;
    }
}