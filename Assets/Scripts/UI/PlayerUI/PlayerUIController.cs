using UnityEngine;

public class PlayerUIController
{
    private PlayerUIView view;
    private PlayerUIModel model;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    public PlayerUIController(PlayerUIView view, PlayerHealth health, PlayerMovement movement)
    {
        this.view = view;
        this.playerHealth = health;
        this.playerMovement = movement;
        this.model = new PlayerUIModel();
    }

    public void Update()
    {
        if (playerHealth == null || playerMovement == null) return;

        model.currentHP = playerHealth.GetCurrentHP();
        model.maxHP = playerHealth.maxHP;
        model.magicCooldownProgress = playerMovement.GetCooldownProgress();
        model.isMagicReady = playerMovement.IsMagicReady();

        UpdateView();
    }

    private void UpdateView()
    {
        view.hpBar.value = (float)model.currentHP / model.maxHP;
        view.hpText.text = $"HP: {model.currentHP}/{model.maxHP}";

        view.magicCooldownImage.fillAmount = model.magicCooldownProgress;
        view.magicCooldownImage.color = model.isMagicReady
            ? Color.white
            : new Color(1, 1, 1, 0.4f);
    }
}