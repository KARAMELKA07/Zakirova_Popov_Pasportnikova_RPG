using UnityEngine;
using System.Collections;

public class RangedEnemyAI : BaseEnemyAI
{
    [System.Serializable]
    public class WeaponSettings
    {
        public GameObject persistentWeapon; 
        public GameObject projectilePrefab; 
        public float cooldown = 3f;
        public float attackDelay = 0.5f;
        public int damage = 25; 
        [Range(0f, 1f)] public float spawnChance = 0.5f;
    }

    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponParent;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private WeaponSettings staffSettings;
    [SerializeField] private WeaponSettings orbSettings;

    [Header("Combat Settings")]
    [SerializeField] private float retreatDistance = 5f;

    private WeaponSettings currentWeapon;
    private float lastAttackTime;
    private GameObject currentWeaponInstance;
    private PlayerHealth playerHealth;

    protected override void Start()
    {
        base.Start();

        if (player == null)
        {
            Debug.LogError("Player не найден!");
            return;
        }

        playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth не найден на игроке!");
            return;
        }

        EquipRandomWeapon();
    }

    private void EquipRandomWeapon()
    {
        if (staffSettings == null || orbSettings == null)
        {
            Debug.LogError("Настройки оружия не назначены!");
            return;
        }

        currentWeapon = Random.value <= 0.5f ? staffSettings : orbSettings;

        // Для посоха
        if (currentWeapon == staffSettings && currentWeapon.persistentWeapon != null && weaponParent != null)
        {
            if (currentWeaponInstance != null)
            {
                Destroy(currentWeaponInstance);
            }
            currentWeaponInstance = Instantiate(
                currentWeapon.persistentWeapon,
                weaponParent.position,
                weaponParent.rotation,
                weaponParent
            );
        }
    }

    protected override void AttackState()
    {
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.position) < retreatDistance)
        {
            FleeState();
            return;
        }

        agent.isStopped = true;
        animator.SetFloat("Speed", 0);

        if (Time.time - lastAttackTime >= currentWeapon.cooldown)
        {
            lastAttackTime = Time.time;
            FacePlayer();
            animator.SetTrigger("attack");
            StartCoroutine(PerformAttackWithDelay());
        }
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private IEnumerator PerformAttackWithDelay()
    {
        yield return new WaitForSeconds(currentWeapon.attackDelay);
        PerformAttack();
    }

    private void PerformAttack()
    {
        if (isDead || player == null) return;

        if (currentWeapon == orbSettings)
        {
            InstantiateProjectile();
        }
        else
        {
            ApplyStaffDamage();
        }
    }

    private void InstantiateProjectile()
    {
        if (currentWeapon.projectilePrefab == null || projectileSpawnPoint == null)
        {
            return;
        }

        GameObject projectile = Instantiate(
            currentWeapon.projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity
        );

        if (projectile.TryGetComponent<MagicProjectile>(out var magicScript))
        {
            Vector3 target = player != null ? player.position + Vector3.up : transform.forward;
            magicScript.SetDirection(target);
        }
    }

    private void ApplyStaffDamage()
    {
        if (player == null || playerHealth == null || currentWeapon == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            playerHealth.TakeDamage(currentWeapon.damage);
        }
    }
}