using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int swordDamage = 30;
    public float attackRange = 2f;
    public LayerMask enemyLayer;
    public int magicDamage = 50;
    public float magicRange = 30f;
    private float magicCooldown = 5f;
    private float magicCooldownTimer = 0f;

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
            MobHealth mobHealth = enemy.GetComponent<MobHealth>();
            if (mobHealth != null)
            {
                mobHealth.TakeDamage(swordDamage);
                Debug.Log($"Мечвом ударил {enemy.name} на {swordDamage} урона!");
            }
        }
    }

    public void DealMagicDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, magicRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            MobHealth mobHealth = enemy.GetComponent<MobHealth>();
            if (mobHealth != null)
            {
                mobHealth.TakeDamage(magicDamage);
                Debug.Log($"Магия ударила {enemy.name} на {magicDamage} урона!");
            }
        }
    }
}