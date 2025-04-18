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
        magicCooldownImage.fillAmount = playerMovement.GetCooldownProgress();

        if (playerMovement.IsMagicReady())
        {
            magicCooldownImage.color = Color.white;
        }
        else
        {
            magicCooldownImage.color = new Color(1, 1, 1, 0.4f);
        }
    }
}