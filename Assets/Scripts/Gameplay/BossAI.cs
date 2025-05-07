using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum BossState { Idle, Aggro, Attack, StrongAttack, SpecialAbility1, Flee }
public enum Element { Fire, Ice, Earth, Ether }
public enum WeaponType { Melee, Ranged }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(AudioSource))]
public class BossAI : MonoBehaviour
{
    [Header("State Settings")]
    public BossState currentState = BossState.Idle;
    public bool isPeaceful = false;
    public float stateChangeInterval = 10f;

    [Header("Combat Settings")]
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public float strongAttackRadius = 1f;
    public float fleeHealthThreshold = 0.2f;
    public float attackCooldown = 3f;
    public float strongAttackCooldown = 7f;
    public int meleeDamage = 30;
    public int rangedDamage = 20;
    public int strongAttackDamage = 40;

    [Header("Ranged Attack Settings")]
    public float minRangedAttackDistance = 10f;
    public float maxRangedAttackDistance = 25f;

    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public ParticleSystem[] elementEffects;
    public AudioClip[] meleeSounds;
    public AudioClip[] rangedSounds;
    public AudioClip[] elementChangeSounds;
    public AudioClip laughSound;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform player;
    protected MobHealth mobHealth;
    protected AudioSource audioSource;
    protected PlayerHealth playerHealth;
    protected bool isDead = false;
    protected float lastAttackTime;
    protected float lastStrongAttackTime;
    protected float lastElementChangeTime;
    protected bool wasMovingBeforeAttack;
    protected Element currentElement;
    protected WeaponType currentWeapon;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mobHealth = GetComponent<MobHealth>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();

        currentWeapon = (WeaponType)Random.Range(0, 2);
        ChangeElement((Element)Random.Range(0, 4));
        lastElementChangeTime = Time.time;

        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
    }

    void Update()
    {
        if (isDead) return;

        if (Time.time - lastElementChangeTime >= stateChangeInterval)
        {
            ChangeElement(GetNextElement());
            lastElementChangeTime = Time.time;
        }

        UpdateState();
        ExecuteState();
    }

    protected virtual void UpdateState()
    {
        if (isPeaceful)
        {
            currentState = BossState.Idle;
            if (mobHealth.GetCurrentHP() <= mobHealth.maxHP * fleeHealthThreshold)
                currentState = BossState.Flee;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRadius)
        {
            currentState = BossState.Idle;
            return;
        }

        float healthPercent = (float)mobHealth.GetCurrentHP() / mobHealth.maxHP;

        if (healthPercent <= fleeHealthThreshold)
        {
            currentState = BossState.Flee;
        }
        else if (distanceToPlayer <= strongAttackRadius && Time.time - lastStrongAttackTime >= strongAttackCooldown)
        {
            currentState = BossState.StrongAttack;
        }
        else if (distanceToPlayer <= attackRadius)
        {
            currentState = BossState.Attack;
        }
        else
        {
            currentState = BossState.Aggro;
        }
    }

    protected virtual void ExecuteState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                IdleState();
                break;
            case BossState.Aggro:
                AggroState();
                break;
            case BossState.Attack:
                AttackState();
                break;
            case BossState.StrongAttack:
                StrongAttackState();
                break;
            case BossState.SpecialAbility1:
                SpecialAbility1State();
                break;
            case BossState.Flee:
                FleeState();
                break;
        }
    }

    protected virtual void AggroState()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool inRangedRange = distance >= minRangedAttackDistance && distance <= maxRangedAttackDistance;
        currentWeapon = inRangedRange ? WeaponType.Ranged : WeaponType.Melee;

        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsWalking", true);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected virtual void IdleState()
    {
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetBool("IsWalking", false);
    }

    protected virtual void AttackState()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool inRangedRange = distance >= minRangedAttackDistance && distance <= maxRangedAttackDistance;

        if (currentWeapon == WeaponType.Ranged && !inRangedRange)
        {
            currentState = BossState.Aggro;
            return;
        }

        wasMovingBeforeAttack = animator.GetBool("IsWalking");
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            FacePlayer();

            if (currentWeapon == WeaponType.Melee)
            {
                animator.SetTrigger("MeleeAttack");
                StartCoroutine(PerformMeleeAttack());
            }
            else if (inRangedRange)
            {
                animator.SetTrigger("RangedAttack");
                StartCoroutine(PerformRangedAttack());
            }
        }
    }

    protected virtual void StrongAttackState()
    {
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);

        if (Time.time - lastStrongAttackTime >= strongAttackCooldown)
        {
            lastStrongAttackTime = Time.time;
            FacePlayer();
            animator.SetTrigger("StrongAttack");
            StartCoroutine(PerformStrongAttack());
        }
    }

    protected virtual void SpecialAbility1State()
    {
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Special1");
        audioSource.PlayOneShot(laughSound);
        StartCoroutine(PerformSpecialAbility1());
    }

    protected virtual void FleeState()
    {
        agent.isStopped = false;
        Vector3 fleeDirection = transform.position - player.position;
        agent.SetDestination(transform.position + fleeDirection.normalized * 10f);
        animator.SetBool("IsWalking", true);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected virtual IEnumerator PerformMeleeAttack()
    {
        yield return new WaitForSeconds(0.3f);
        if (isDead) yield break;

        if (PlayerInRange(attackRadius))
        {
            playerHealth.TakeDamage(meleeDamage);
            PlayElementEffect();
            PlayWeaponSound(true);
        }
        yield return new WaitForSeconds(1.2f - 0.3f);
        ReturnToPreviousState();
    }

    protected virtual IEnumerator PerformRangedAttack()
    {
        yield return new WaitForSeconds(0.5f);
        if (isDead) yield break;

        PlayElementEffect();
        PlayWeaponSound(false);
        SpawnProjectile();
        yield return new WaitForSeconds(1.5f - 0.5f);
        ReturnToPreviousState();
    }

    protected virtual IEnumerator PerformStrongAttack()
    {
        yield return new WaitForSeconds(0.8f);
        if (isDead) yield break;

        if (PlayerInRange(strongAttackRadius))
        {
            playerHealth.TakeDamage(strongAttackDamage);
            PlayElementEffect();
            PlayWeaponSound(true);
        }
        yield return new WaitForSeconds(2f - 0.8f);
        ReturnToPreviousState();
    }

    protected virtual IEnumerator PerformSpecialAbility1()
    {
        yield return new WaitForSeconds(2.5f);
        currentState = BossState.Idle;
    }

    protected void ReturnToPreviousState()
    {
        if (isDead) return;
        animator.SetBool("IsWalking", wasMovingBeforeAttack);
        currentState = wasMovingBeforeAttack ? BossState.Aggro : BossState.Idle;
    }

    protected void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    protected bool PlayerInRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.position) <= range;
    }

    protected void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        BossProjectile proj = projectile.GetComponent<BossProjectile>();
        if (proj != null)
        {
            proj.Setup(
                currentElement,
                rangedDamage,
                (player.position - firePoint.position).normalized
            );
        }
    }

    protected Element GetNextElement()
    {
        return (Element)(((int)currentElement + 1) % 4);
    }

    protected void ChangeElement(Element newElement)
    {
        currentElement = newElement;
        currentState = BossState.SpecialAbility1;
        PlayElementChangeSound();
    }

    protected void PlayElementEffect()
    {
        if ((int)currentElement < elementEffects.Length && elementEffects[(int)currentElement] != null)
            elementEffects[(int)currentElement].Play();
    }

    protected void PlayWeaponSound(bool isMelee)
    {
        AudioClip[] clips = isMelee ? meleeSounds : rangedSounds;
        if (clips.Length > 0)
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    protected void PlayElementChangeSound()
    {
        if (elementChangeSounds.Length > 0)
            audioSource.PlayOneShot(elementChangeSounds[Random.Range(0, elementChangeSounds.Length)]);
    }

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        foreach (var collider in GetComponents<Collider>())
            collider.enabled = false;
        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, strongAttackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minRangedAttackDistance);
        Gizmos.DrawWireSphere(transform.position, maxRangedAttackDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}