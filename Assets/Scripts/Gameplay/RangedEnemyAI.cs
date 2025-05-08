using UnityEngine;

public class RangedEnemyAI : BaseEnemyAI
{
    [Header("Ranged Attack Settings")]
    public float retreatDistance = 5f;
    public float attackCooldown = 3f;
    public GameObject magicProjectilePrefab;
    public Transform firePoint;

    private float lastAttackTime;

    protected override void AttackState()
    {
        // Отступаем если слишком близко
        if (Vector3.Distance(transform.position, player.position) < retreatDistance)
        {
            FleeState();
            return;
        }

        agent.isStopped = true;
        animator.SetFloat("Speed", 0);

        // Атакуем если прошло достаточно времени
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetTrigger("attack");
            Invoke("Shoot", 0.5f);
        }
    }

    private void Shoot()
    {
        if (isDead || player == null) return;

        Vector3 targetPosition = player.position;
        GameObject projectile = Instantiate(magicProjectilePrefab, firePoint.position, Quaternion.identity);
        MagicProjectile magicScript = projectile.GetComponent<MagicProjectile>();

        if (magicScript != null)
        {
            magicScript.SetDirection(targetPosition);
        }
        else
        {
            Debug.LogError("У снаряда отсутствует компонент MagicProjectile!");
        }
    }
}