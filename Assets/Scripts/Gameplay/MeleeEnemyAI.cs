using UnityEngine;
using System.Collections;

public class MeleeEnemyAI : BaseEnemyAI
{
    [Header("Melee Attack Settings")]
    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;
    public int attackDamage = 35;

    private float lastAttackTime;
    private PlayerHealth playerHealth;

    protected override void Start()
    {
        base.Start();
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    protected override void AttackState()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetTrigger("attack");
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(attackDelay);

        if (isDead) yield break;

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}