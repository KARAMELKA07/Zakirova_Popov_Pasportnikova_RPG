using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum BossState { Idle, Aggro, Attack, StrongAttack, SpecialAbility1, SpecialAbility2, Flee }
public enum Element { Fire, Ice, Earth, Ether }
public enum WeaponType { Melee, Ranged }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(AudioSource))]
public class BossAI : MonoBehaviour
{
    [Header("State Machine")]
    public BossState currentState = BossState.Idle;
    public bool isPeaceful = false;
    public float stateChangeInterval = 10f;

    [Header("Combat Settings")]
    public float detectionRadius = 15f;
    public float attackRadius = 5f;
    public float strongAttackRadius = 3f;
    public float fleeHealthThreshold = 0.2f;
    public float attackCooldown = 2f;
    public float strongAttackCooldown = 5f;
    public int meleeDamage = 30;
    public int rangedDamage = 20;
    public int strongAttackDamage = 50;
    [Range(0, 100)] public float etherBackwardChance = 30f;

    [Header("Elements & Weapons")]
    public Element currentElement;
    public WeaponType currentWeapon;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Animation Settings")]
    public float meleeAttackAnimLength = 1.2f;
    public float rangedAttackAnimLength = 1.5f;
    public float strongAttackAnimLength = 2f;
    public float special1AnimLength = 2.5f; // Для смеха
    public float special2AnimLength = 3f;   // Для ходьбы задом
    public float dieAnimLength = 3f;

    [Header("Effects")]
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

        // Настройка для ходьбы задом
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
        float healthPercent = (float)mobHealth.GetCurrentHP() / mobHealth.maxHP;

        if (healthPercent <= fleeHealthThreshold)
            currentState = BossState.Flee;
        else if (distanceToPlayer <= strongAttackRadius && Time.time - lastStrongAttackTime >= strongAttackCooldown)
            currentState = BossState.StrongAttack;
        else if (distanceToPlayer <= attackRadius)
            currentState = BossState.Attack;
        else if (distanceToPlayer <= detectionRadius)
            currentState = BossState.Aggro;
        else
            currentState = BossState.Idle;
    }

    protected virtual void ExecuteState()
    {
        switch (currentState)
        {
            case BossState.Idle: IdleState(); break;
            case BossState.Aggro: AggroState(); break;
            case BossState.Attack: AttackState(); break;
            case BossState.StrongAttack: StrongAttackState(); break;
            case BossState.SpecialAbility1: SpecialAbility1State(); break;
            case BossState.SpecialAbility2: SpecialAbility2State(); break;
            case BossState.Flee: FleeState(); break;
        }
    }

    protected virtual void IdleState()
    {
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsWalkingBackwards", false);
    }

    protected virtual void AggroState()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsWalkingBackwards", false);
        animator.SetFloat("Speed", agent.velocity.magnitude);
        currentWeapon = Vector3.Distance(transform.position, player.position) > attackRadius
            ? WeaponType.Ranged : WeaponType.Melee;
    }

    protected virtual void AttackState()
    {
        if (currentElement == Element.Ether && Random.Range(0, 100) < etherBackwardChance)
        {
            currentState = BossState.SpecialAbility2;
            return;
        }

        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsWalkingBackwards", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            FacePlayer();

            if (currentWeapon == WeaponType.Melee)
            {
                animator.SetTrigger("MeleeAttack");
                StartCoroutine(PerformMeleeAttack());
            }
            else
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
        animator.SetBool("IsWalkingBackwards", false);

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
        // Злобный смех при смене стихии
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsWalkingBackwards", false);
        animator.SetTrigger("Special1");
        audioSource.PlayOneShot(laughSound);
        StartCoroutine(PerformSpecialAbility1());
    }

    protected virtual void SpecialAbility2State()
    {
        // Ходьба задом для эфира
        if (currentElement == Element.Ether)
        {
            agent.isStopped = false;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsWalkingBackwards", true);
            Vector3 retreatDirection = -transform.forward;
            agent.SetDestination(transform.position + retreatDirection * 3f);
            StartCoroutine(PerformSpecialAbility2());
        }
    }

    protected virtual void FleeState()
    {
        agent.isStopped = false;
        Vector3 fleeDirection = transform.position - player.position;
        agent.SetDestination(transform.position + fleeDirection.normalized * 10f);
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsWalkingBackwards", false);
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
        yield return new WaitForSeconds(meleeAttackAnimLength - 0.3f);
    }

    protected virtual IEnumerator PerformRangedAttack()
    {
        yield return new WaitForSeconds(0.5f);
        if (isDead) yield break;

        PlayElementEffect();
        PlayWeaponSound(false);
        SpawnProjectile();
        yield return new WaitForSeconds(rangedAttackAnimLength - 0.5f);
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
        yield return new WaitForSeconds(strongAttackAnimLength - 0.8f);
    }

    protected virtual IEnumerator PerformSpecialAbility1()
    {
        yield return new WaitForSeconds(special1AnimLength);
        currentState = BossState.Idle;
    }

    protected virtual IEnumerator PerformSpecialAbility2()
    {
        yield return new WaitForSeconds(special2AnimLength);
        animator.SetBool("IsWalkingBackwards", false);
        currentState = BossState.Attack;
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
        else
        {
            Debug.LogError("BossProjectile component missing on projectile!");
        }
    }

    protected Element GetNextElement() => (Element)(((int)currentElement + 1) % 4);

    protected void ChangeElement(Element newElement)
    {
        currentElement = newElement;
        currentState = BossState.SpecialAbility1; // Активируем смех
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
        Destroy(gameObject, dieAnimLength);
    }
}