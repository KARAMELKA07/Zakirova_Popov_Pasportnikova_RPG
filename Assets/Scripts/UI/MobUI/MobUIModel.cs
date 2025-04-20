public class MobUIModel
{
    public int maxHP;
    public int currentHP;
    public float hpPercent => maxHP > 0 ? (float)currentHP / maxHP : 0f;
}