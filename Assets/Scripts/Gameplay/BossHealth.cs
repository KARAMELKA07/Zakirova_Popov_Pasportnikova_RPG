using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHP = 150;
    public float peacefulDuration = 2f;

    private int currentHP;
    private BossAI bossAI;
    private bool isDead = false;
    private float peacefulTimer = 0f;
    private bool isPeacefulAfterDamage = false;

    void Start()
    {
        currentHP = maxHP;
        bossAI = GetComponent<BossAI>();
        tag = "Boss";
        bossAI.SetPeaceful(false);
        isPeacefulAfterDamage = false;
    }

    void Update()
    {
        if (isPeacefulAfterDamage)
        {
            peacefulTimer -= Time.deltaTime;
            if (peacefulTimer <= 0f)
            {
                isPeacefulAfterDamage = false;
                bossAI.SetPeaceful(false);
                Debug.Log("[BossHealth] Мирный режим после урона закончился");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || bossAI == null) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        Debug.Log($"[BossHealth] Получено {damage} урона. Осталось HP: {currentHP}");

        // Активируем мирный режим
        bossAI.SetPeaceful(true);
        isPeacefulAfterDamage = true;
        peacefulTimer = peacefulDuration;

        if (currentHP <= 0)
        {
            isDead = true;
            bossAI.Die();
        }
    }

    public int GetCurrentHP() => currentHP;

    public void ForceSetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
    }
}