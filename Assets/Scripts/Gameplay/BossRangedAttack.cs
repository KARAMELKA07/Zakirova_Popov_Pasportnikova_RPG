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

        Vector3 playerChestPosition = player.position + Vector3.up * 0.2f; 

        Vector3 direction = (playerChestPosition - firePoint.position).normalized;

        direction += Vector3.up * 0.05f; 
        direction = direction.normalized; 

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
        // Получаем индекс текущего элемента
        int elementIndex = (int)bossAI.currentElement;

        // Проверяем, что есть звуки для этого элемента
        if (bossAI.rangedSounds != null && bossAI.rangedSounds.Length > elementIndex)
        {
            AudioClip elementSound = bossAI.rangedSounds[elementIndex];
            if (elementSound != null)
            {
                audioSource.PlayOneShot(elementSound);
            }
            else
            {
                Debug.LogWarning($"No ranged sound for element {bossAI.currentElement}");
            }
        }
        else
        {
            Debug.LogError("Ranged sounds array is not properly configured for all elements");
        }
    }
}