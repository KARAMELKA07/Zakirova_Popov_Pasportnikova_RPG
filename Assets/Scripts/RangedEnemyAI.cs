using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float attackRadius = 10f;
    public float retreatDistance = 5f;
    public float attackCooldown = 3f;
    public GameObject magicProjectilePrefab;
    public Transform firePoint; 

    private NavMeshAgent agent;
    private Transform player;
    private float lastAttackTime;

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
            if (distanceToPlayer < retreatDistance)
            {
                // Отходим назад
                Vector3 dirToPlayer = transform.position - player.position;
                Vector3 retreatPos = transform.position + dirToPlayer.normalized * 2f;
                agent.SetDestination(retreatPos);
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Маг атакует!");

            // Создаем снаряд
            GameObject projectile = Instantiate(magicProjectilePrefab, firePoint.position, Quaternion.identity);
            MagicProjectile magicScript = projectile.GetComponent<MagicProjectile>();
            magicScript.SetTarget(player);
        }
    }
}