using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;
    private Animator animator;

    public GameObject gameOverPanel; // Панель "Game Over"
    public Text gameOverText;

    void Start()
    {
        currentHP = maxHP;
        gameOverPanel.SetActive(false); // Скрываем при старте
        animator = GetComponent<Animator>();
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Игрок получил {damage} урона. HP осталось: {currentHP}");
        animator.SetTrigger("TakeDamage");

        if (currentHP <= 0)
        {
            currentHP = 0;
            animator.SetBool("IsDead", true); // Устанавливаем флаг для анимации смерти

            // Запускаем корутину для задержки перед смертью
            StartCoroutine(DeathSequence());
        }
    }

    // Корутину для задержки перед смертью
    private IEnumerator DeathSequence()
    {
        // Задержка на время анимации смерти (например, 2 секунды)
        yield return new WaitForSeconds(2f);

        // Останавливаем движение и анимацию
        animator.SetTrigger("Die");

        // Появление панели "Game Over"
        Die();
    }

    void Die()
    {
        Debug.Log("Игрок умер.");

        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
