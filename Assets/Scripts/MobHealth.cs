using UnityEngine;

public class MobHealth : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} помер(");
        MobManager.Instance.UnregisterMob(gameObject);
    }
}