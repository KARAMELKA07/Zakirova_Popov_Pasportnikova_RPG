using UnityEngine;

public class MobUIController
{
    private MobUIView view;
    private MobUIModel model;
    private MobHealth mobHealth;

    public MobUIController(MobUIView view, MobHealth mobHealth)
    {
        this.view = view;
        this.mobHealth = mobHealth;
        this.model = new MobUIModel
        {
            maxHP = mobHealth.maxHP
        };
    }

    public void Update()
    {
        model.currentHP = mobHealth.GetCurrentHP();
        view.UpdateHP(model.hpPercent);
        view.UpdatePosition();
    }
}