using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BossAI))]
public class BossRangedAttack : MonoBehaviour
{
    [Header("Ranged Attack Settings")]
    public float minRangedAttackDistance = 15f;
    public float maxRangedAttackDistance = 20f;
    public float rangedAttackCooldown = 3f;
    public GameObject[] projectilePrefabs; // 4 префаба для разных стихий
    public Transform firePoint;

    private BossAI bossAI;
    private float lastRangedAttackTime;
    private Animator animator;
    private AudioSource audioSource;
    private Transform player;

    void Awake()
    {
        bossAI = GetComponent<BossAI>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool CanPerformRangedAttack()
    {
        if (Time.time - lastRangedAttackTime < rangedAttackCooldown)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance >= minRangedAttackDistance && distance <= maxRangedAttackDistance;
    }

    public void PerformRangedAttack()
    {
        lastRangedAttackTime = Time.time;
        animator.SetTrigger("RangedAttack");
        StartCoroutine(ExecuteRangedAttack());
    }

    private IEnumerator ExecuteRangedAttack()
    {
        yield return new WaitForSeconds(0.5f);

        if (bossAI.isDead) yield break;

        SpawnProjectile();
        PlayElementEffect();
        PlayAttackSound();

        yield return new WaitForSeconds(1.0f);
    }

    private void SpawnProjectile()
    {
        if (projectilePrefabs.Length != 4)
        {
            Debug.LogError("Need 4 projectile prefabs for all elements!");
            return;
        }

        int elementIndex = (int)bossAI.currentElement;
        GameObject projectilePrefab = projectilePrefabs[elementIndex];

        if (projectilePrefab == null)
        {
            Debug.LogError($"Projectile prefab for {bossAI.currentElement} is missing!");
            return;
        }

        // Получаем позицию груди игрока (примерно на половине высоты)
        Vector3 playerChestPosition = player.position + Vector3.up * 0.2f; // 1.0f - примерная высота груди

        // Рассчитываем направление с небольшим смещением вверх
        Vector3 direction = (playerChestPosition - firePoint.position).normalized;

        // Добавляем дополнительное смещение вверх (можно регулировать)
        direction += Vector3.up * 0.1f; // 0.2f - коэффициент высоты полета
        direction = direction.normalized; // Нормализуем после добавления смещения

        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rotation);
        BossProjectile proj = projectile.GetComponent<BossProjectile>();

        if (proj != null)
        {
            proj.Setup(
                bossAI.currentElement,
                bossAI.rangedDamage,
                direction
            );
        }
    }

    private void PlayElementEffect()
    {
        if ((int)bossAI.currentElement < bossAI.elementEffects.Length &&
            bossAI.elementEffects[(int)bossAI.currentElement] != null)
        {
            bossAI.elementEffects[(int)bossAI.currentElement].Play();
        }
    }

    private void PlayAttackSound()
    {
        if (bossAI.rangedSounds.Length > 0)
        {
            audioSource.PlayOneShot(bossAI.rangedSounds[Random.Range(0, bossAI.rangedSounds.Length)]);
        }
    }
}