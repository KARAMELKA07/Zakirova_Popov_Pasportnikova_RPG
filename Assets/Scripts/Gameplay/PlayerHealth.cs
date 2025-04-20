using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;
    private Animator animator;

    public GameObject gameOverPanel; 
    public Text gameOverText;

    void Start()
    {
        currentHP = maxHP;
        gameOverPanel.SetActive(false); 
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
            animator.SetBool("IsDead", true); 

            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);

        animator.SetTrigger("Die");

        Die();
    }

    void Die()
    {
        Debug.Log("Игрок умер.");

        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over!";
    }
    
    public void ForceSetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
