using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHP = 150;
    private int currentHP;
    private MeleeEnemyAI meleeEnemyAI;
    private RangedEnemyAI rangedEnemyAI;
    private BossAI bossAI; // Добавлена ссылка на BossAI

    void Start()
    {
        currentHP = maxHP;
        meleeEnemyAI = GetComponent<MeleeEnemyAI>();
        rangedEnemyAI = GetComponent<RangedEnemyAI>();
        bossAI = GetComponent<BossAI>(); // Инициализация BossAI
    }

    public void TakeDamage(int damage)
    {
        // Отключаем мирный режим при получении урона (если это босс)
        if (bossAI != null && bossAI.isPeaceful)
        {
            bossAI.isPeaceful = false;
        }

        currentHP -= damage;
        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось HP: {currentHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} помер(");

        if (meleeEnemyAI != null)
        {
            meleeEnemyAI.Die();
        }
        else if (rangedEnemyAI != null)
        {
            rangedEnemyAI.Die();
        }

        MobManager.Instance.UnregisterMob(gameObject);
    }

    public void ForceSetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
    }
}