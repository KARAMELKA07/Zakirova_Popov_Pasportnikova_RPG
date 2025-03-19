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
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead) return; // Если враг мертв, он не выполняет никаких действий

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (distanceToPlayer <= attackRadius)
        {
            AttackPlayer();
            if (distanceToPlayer < retreatDistance)
            {
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
        if (isDead || Time.time - lastAttackTime < attackCooldown || player == null) return;

        lastAttackTime = Time.time;
        agent.SetDestination(transform.position); // Останавливаем движение

        // Поворачиваем мага в сторону игрока
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Убираем наклон по оси Y
        transform.rotation = Quaternion.LookRotation(direction);

        animator.SetTrigger("attack"); // Запускаем анимацию атаки

        Invoke("Shoot", 0.5f); // Добавляем небольшую задержку перед выстрелом
    }

    void Shoot()
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

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"{gameObject.name} умер.");
        animator.SetTrigger("die"); // Запускаем анимацию смерти
        agent.isStopped = true; // Останавливаем передвижение
        agent.enabled = false; // Отключаем NavMeshAgent
        Destroy(gameObject, 3f); // Удаляем объект после задержки
    }
}
