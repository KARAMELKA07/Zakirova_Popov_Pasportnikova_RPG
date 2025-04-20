using UnityEngine;

public class MobHealth : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;
    private MeleeEnemyAI meleeEnemyAI;
    private RangedEnemyAI rangedEnemyAI;

    void Start()
    {
        currentHP = maxHP;
        meleeEnemyAI = GetComponent<MeleeEnemyAI>();
        rangedEnemyAI = GetComponent<RangedEnemyAI>();
    }

    public void TakeDamage(int damage)
    {
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
