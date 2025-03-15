using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Health")]
    public Slider hpBar;
    public Text hpText;
    public PlayerHealth playerHealth;

    [Header("Magic Cooldown")]
    public Image magicCooldownImage;
    public PlayerMovement playerMovement;

    void Update()
    {
        UpdateHealth();
        UpdateMagicCooldown();
    }

    void UpdateHealth()
    {
        hpBar.value = (float)playerHealth.GetCurrentHP() / playerHealth.maxHP;
        hpText.text = $"HP: {playerHealth.GetCurrentHP()}/{playerHealth.maxHP}";
    }

    void UpdateMagicCooldown()
    {
        // Обновление заполнения
        magicCooldownImage.fillAmount = playerMovement.GetCooldownProgress();

        // Управление видимостью
        if (playerMovement.IsMagicReady())
        {
            // Полная видимость + эффект готовности
            magicCooldownImage.color = Color.white;
        }
        else
        {
            // Полупрозрачность во время перезарядки
            magicCooldownImage.color = new Color(1, 1, 1, 0.4f);
        }
    }
}