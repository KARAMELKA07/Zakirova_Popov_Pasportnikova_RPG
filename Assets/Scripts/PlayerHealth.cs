using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    public GameObject gameOverPanel; // Панель "Game Over"
    public Text gameOverText;

    void Start()
    {
        currentHP = maxHP;
        gameOverPanel.SetActive(false); // Скрываем при старте
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Игрок получил {damage} урона. HP осталось: {currentHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("помер(");
        gameOverPanel.SetActive(true);
        gameOverText.text = "помер";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}