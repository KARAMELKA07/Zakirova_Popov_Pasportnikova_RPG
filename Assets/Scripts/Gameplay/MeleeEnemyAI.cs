using UnityEngine;
using System.Collections;

public class MeleeEnemyAI : BaseEnemyAI
{
    [System.Serializable]
    public class WeaponSettings
    {
        public GameObject weaponPrefab;
        public int damage = 35;
        public float cooldown = 2f;
        public float attackDelay = 0.5f;
        [Range(0f, 1f)] public float spawnChance = 0.5f;
    }

    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponParent;
    [SerializeField] private WeaponSettings axeSettings;
    [SerializeField] private WeaponSettings knifeSettings;

    private WeaponSettings currentWeapon;
    private float lastAttackTime;
    private PlayerHealth playerHealth;
    private GameObject currentWeaponInstance;

    protected override void Start()
    {
        base.Start();
        playerHealth = player.GetComponent<PlayerHealth>();
        EquipRandomWeapon();
    }

    private void EquipRandomWeapon()
    {
        float random = Random.value;
        currentWeapon = random <= axeSettings.spawnChance ? axeSettings : knifeSettings;

        if (currentWeapon.weaponPrefab && weaponParent)
        {
            currentWeaponInstance = Instantiate(
                currentWeapon.weaponPrefab,
                weaponParent.position,
                weaponParent.rotation,
                weaponParent
            );
        }
    }

    protected override void AttackState()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);

        if (Time.time - lastAttackTime >= currentWeapon.cooldown)
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
        yield return new WaitForSeconds(currentWeapon.attackDelay);

        if (isDead) yield break;

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            playerHealth.TakeDamage(currentWeapon.damage);
        }
    }


}