using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Aggro, Attack, Flee }

public abstract class BaseEnemyAI : MonoBehaviour
{
    [Header("State Machine")]
    public EnemyState currentState = EnemyState.Idle;
    public bool isPeaceful = false;

    [Header("Common Settings")]
    public float detectionRadius = 10f;
    public float attackRadius = 3f;
    public float fleeHealthThreshold = 0.3f;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform player;
    protected MobHealth mobHealth;
    protected bool isDead = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mobHealth = GetComponent<MobHealth>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        UpdateState();
        ExecuteState();
    }

    protected virtual void UpdateState()
    {
        if (isPeaceful)
        {
            currentState = EnemyState.Idle;

            // Если здоровье низкое - бежим
            if (mobHealth.GetCurrentHP() <= mobHealth.maxHP * fleeHealthThreshold)
            {
                currentState = EnemyState.Flee;
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float healthPercent = (float)mobHealth.GetCurrentHP() / mobHealth.maxHP;

        // Бегство при низком здоровье
        if (healthPercent <= fleeHealthThreshold)
        {
            currentState = EnemyState.Flee;
        }
        // Атака если в радиусе атаки
        else if (distanceToPlayer <= attackRadius)
        {
            currentState = EnemyState.Attack;
        }
        // Преследование если видим игрока
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Aggro;
        }
        // Иначе покой
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    protected virtual void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Aggro:
                AggroState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
            case EnemyState.Flee:
                FleeState();
                break;
        }
    }

    protected virtual void IdleState()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
    }

    protected virtual void AggroState()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected abstract void AttackState();

    protected virtual void FleeState()
    {
        agent.isStopped = false;
        Vector3 fleeDirection = transform.position - player.position;
        Vector3 fleePosition = transform.position + fleeDirection.normalized * 5f;
        agent.SetDestination(fleePosition);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("die");
        agent.isStopped = true;
        agent.enabled = false;

        Destroy(gameObject, 3f);
    }
}