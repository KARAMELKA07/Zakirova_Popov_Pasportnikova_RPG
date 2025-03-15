using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    public GameObject magicSphere;
    public Animator magicAnimator;
    public float magicAttackCooldown = 5f;

    private float magicCooldownTimer;
    private bool isMagicOnCooldown;

    void Update()
    {
        UpdateMagicCooldown();
    }

    public void TriggerMagicAttack()
    {
        if (isMagicOnCooldown) return;

        // Активируем сферу и запускаем анимацию
        magicSphere.SetActive(true);
        magicAnimator.SetTrigger("MagicAttack");

        // Запускаем кулдаун
        isMagicOnCooldown = true;
        magicCooldownTimer = magicAttackCooldown;

        // Выключаем сферу после окончания анимации
        Invoke(nameof(ResetMagicSphere), magicAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    void ResetMagicSphere()
    {
        magicSphere.SetActive(false);
    }

    void UpdateMagicCooldown()
    {
        if (isMagicOnCooldown)
        {
            magicCooldownTimer -= Time.deltaTime;
            if (magicCooldownTimer <= 0f)
            {
                isMagicOnCooldown = false;
            }
        }
    }

    // Добавляем метод для UI
    public float GetCooldownProgress()
    {
        return isMagicOnCooldown ? 1 - (magicCooldownTimer / magicAttackCooldown) : 1f;
    }

    // Добавляем метод для UI
    public bool IsMagicReady()
    {
        return !isMagicOnCooldown;
    }
}
