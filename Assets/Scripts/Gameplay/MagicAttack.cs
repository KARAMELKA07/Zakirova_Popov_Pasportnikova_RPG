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

        magicSphere.SetActive(true);
        magicAnimator.SetTrigger("MagicAttack");

        isMagicOnCooldown = true;
        magicCooldownTimer = magicAttackCooldown;

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

    public float GetCooldownProgress()
    {
        return isMagicOnCooldown ? 1 - (magicCooldownTimer / magicAttackCooldown) : 1f;
    }

    public bool IsMagicReady()
    {
        return !isMagicOnCooldown;
    }
}
