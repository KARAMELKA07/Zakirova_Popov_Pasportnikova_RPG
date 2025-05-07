using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Sword Settings")]
    public int swordDamage = 30;
    public float attackRange = 2f;
    public LayerMask enemyLayer;

    [Header("Magic Settings")]
    public int magicDamage = 50;
    public float magicRange = 30f;
    private float magicCooldown = 5f;
    private float magicCooldownTimer = 0f;

    [Header("Boss Settings")]
    public string bossTag = "Boss"; // Тег для идентификации босса
    public int bossSwordDamageMultiplier = 2; // Множитель урона меча по боссу
    public int bossMagicDamageMultiplier = 3; // Множитель урона магии по боссу

    void Update()
    {
        if (magicCooldownTimer > 0)
        {
            magicCooldownTimer -= Time.deltaTime;
        }
    }

    public void DealSwordDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Пробуем получить оба типа здоровья
            BossHealth bossHealth = enemy.GetComponent<BossHealth>();
            MobHealth mobHealth = enemy.GetComponent<MobHealth>();

            if (bossHealth != null)
            {
                int finalDamage = swordDamage;
                if (enemy.CompareTag(bossTag))
                {
                    finalDamage *= bossSwordDamageMultiplier;
                    Debug.Log($"Удар мечом по БОССУ! Урон: {finalDamage}");
                }
                bossHealth.TakeDamage(finalDamage);
            }
            else if (mobHealth != null)
            {
                int finalDamage = swordDamage;
                Debug.Log($"Удар мечом по врагу {enemy.name}. Урон: {finalDamage}");
                mobHealth.TakeDamage(finalDamage);
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
            MobHealth mobHealth = enemy.GetComponent<MobHealth>();
            if (mobHealth != null)
            {
                int finalDamage = magicDamage;

                if (enemy.CompareTag(bossTag))
                {
                    finalDamage *= bossMagicDamageMultiplier;
                    Debug.Log($"Магическая атака по БОССУ! Урон: {finalDamage}");
                }
                else
                {
                    Debug.Log($"Магическая атака по врагу {enemy.name}. Урон: {finalDamage}");
                }

                mobHealth.TakeDamage(finalDamage);
            }
        }

        magicCooldownTimer = magicCooldown;
    }

    // Визуализация радиуса атаки в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magicRange);
    }
}