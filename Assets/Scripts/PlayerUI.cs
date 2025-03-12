using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider hpBar;
    public Text hpText;
    public PlayerHealth playerHealth;
    private float maxMagicCooldown = 5f;

    void Update()
    {
        hpBar.value = (float)playerHealth.GetCurrentHP() / playerHealth.maxHP;
        hpText.text = $"HP: {playerHealth.GetCurrentHP()} / {playerHealth.maxHP}";
        
    }
    
}