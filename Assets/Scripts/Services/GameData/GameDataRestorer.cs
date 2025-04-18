using UnityEngine;

public static class GameDataRestorer
{
    public static void Restore(GameData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosition;

            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ForceSetHP(data.playerHP);
            }
        }

        // Удаляем старых мобов
        foreach (var mob in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            GameObject.Destroy(mob);
        }

        // Спавним мобов из сохранения
        foreach (var mobData in data.mobs)
        {
            GameObject prefab = Resources.Load<GameObject>($"Mobs/{mobData.mobType}");
            if (prefab != null)
            {
                GameObject mob = GameObject.Instantiate(prefab, mobData.position, Quaternion.identity);
                var mobHealth = mob.GetComponent<MobHealth>();
                if (mobHealth != null)
                {
                    mobHealth.ForceSetHP(mobData.hp);
                }
                MobManager.Instance.RegisterMob(mob);
            }
        }
    }
}