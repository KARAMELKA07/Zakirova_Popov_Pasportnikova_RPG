using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    public float detectionRadius = 10f;
    public float attackRadius = 1.5f;
    public float forgetRadius = 15f;
    public float attackCooldown = 2f;
    public int attackDamage = 35;

    private NavMeshAgent agent;
    private Transform player;
    private float lastAttackTime = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.SetDestination(player.position); // Преследуем игрока
        }
        else if (distanceToPlayer > forgetRadius)
        {
            agent.SetDestination(transform.position); // Останавливаемся
        }
    }

    void AttackPlayer()
    {
        // Проверка, что у игрока есть здоровье больше 0
        if (Time.time - lastAttackTime >= attackCooldown && player.GetComponent<PlayerHealth>().GetCurrentHP() > 0)
        {
            lastAttackTime = Time.time;
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }
}
