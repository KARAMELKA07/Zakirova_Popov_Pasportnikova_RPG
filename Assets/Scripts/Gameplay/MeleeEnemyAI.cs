using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MeleeEnemyAI : MonoBehaviour
{
    public float detectionRadius = 10f;
    public float attackRadius = 3f;
    public float forgetRadius = 15f;
    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;
    public int attackDamage = 35;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private float lastAttackTime = 0;
    private bool isDead = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (isDead) return; 
        if (playerHealth == null || playerHealth.GetCurrentHP() <= 0)
        {
            StopChasing();
            animator.SetFloat("Speed", 0);
            return; 
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer > forgetRadius)
        {
            StopChasing();
        }

        float speed = agent.desiredVelocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    void ChasePlayer()
    {
        if (isDead) return;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void StopChasing()
    {
        if (isDead) return;
        agent.isStopped = true;
    }

    void AttackPlayer()
    {
        if (isDead) return;

        if (Time.time - lastAttackTime >= attackCooldown && player.GetComponent<PlayerHealth>().GetCurrentHP() > 0)
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
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<NavMeshAgent>().enabled = false;
        animator.SetTrigger("Die"); 

        Destroy(gameObject, 3f); 
    }

    

}
