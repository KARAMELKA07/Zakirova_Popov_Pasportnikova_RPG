using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Sword Settings")]
    public int swordDamage = 30;
    public float attackRange = 2f;
    public float attackAngle = 90f; // Угол атаки
    public LayerMask enemyLayer;

    [Header("Magic Settings")]
    public int magicDamage = 50;
    public float magicRange = 30f;
    private float magicCooldown = 5f;
    private float magicCooldownTimer = 0f;

    [Header("Boss Settings")]
    public string bossTag = "Boss";
    public int bossSwordDamageMultiplier = 2;
    public int bossMagicDamageMultiplier = 3;
    public float bossHitRadius = 3f; // Отдельный радиус для босса

    void Update()
    {
        if (magicCooldownTimer > 0)
        {
            magicCooldownTimer -= Time.deltaTime;
        }
    }

    public void DealSwordDamage()
    {
        // Проверяем обычных врагов
        CheckRegularEnemies();

        // Отдельная проверка для босса
        CheckBossHit();
    }

    void CheckRegularEnemies()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag(bossTag)) continue; // Пропускаем босса

            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle <= attackAngle / 2)
            {
                MobHealth mobHealth = enemy.GetComponent<MobHealth>();
                if (mobHealth != null)
                {
                    Debug.Log($"Удар мечом по врагу {enemy.name}. Урон: {swordDamage}");
                    mobHealth.TakeDamage(swordDamage);
                }
            }
        }
    }

    void CheckBossHit()
    {
        GameObject boss = GameObject.FindGameObjectWithTag(bossTag);
        if (boss == null) return;

        float distance = Vector3.Distance(transform.position, boss.transform.position);
        if (distance <= bossHitRadius)
        {
            Vector3 directionToBoss = (boss.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToBoss);

            if (angle <= attackAngle / 2)
            {
                BossHealth bossHealth = boss.GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    int finalDamage = swordDamage * bossSwordDamageMultiplier;
                    Debug.Log($"Удар мечом по БОССУ! Урон: {finalDamage}");
                    bossHealth.TakeDamage(finalDamage);
                }
            }
        }
    }

    public void DealMagicDamage()
    {
        if (magicCooldownTimer > 0)
        {
            Debug.Log("Магия ещё на перезарядке!");
            return;
        }

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, magicRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            int finalDamage = magicDamage;

            if (enemy.CompareTag(bossTag))
            {
                BossHealth bossHealth = enemy.GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    finalDamage *= bossMagicDamageMultiplier;
                    Debug.Log($"Магическая атака по БОССУ! Урон: {finalDamage}");
                    bossHealth.TakeDamage(finalDamage);
                }
            }
            else
            {
                MobHealth mobHealth = enemy.GetComponent<MobHealth>();
                if (mobHealth != null)
                {
                    Debug.Log($"Магическая атака по врагу {enemy.name}. Урон: {finalDamage}");
                    mobHealth.TakeDamage(finalDamage);
                }
            }
        }

        magicCooldownTimer = magicCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bossHitRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magicRange);
    }
}