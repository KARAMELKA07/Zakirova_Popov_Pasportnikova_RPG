using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Игрок получил {damage} урона. HP осталось: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("помер");
    }
}