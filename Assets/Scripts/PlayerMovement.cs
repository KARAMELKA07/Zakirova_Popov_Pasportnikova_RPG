using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public Transform cameraTransform;
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float turnSmoothTime = 0.1f;

    [Header("Magic Attack")]
    public float magicAttackCooldown = 5f;
    public Animator animator;
    public GameObject magicSphere;
    public Transform magicSphereStartPos;
    public float sphereAnimationDuration = 1.5f;

    private float turnSmoothVelocity;
    private bool isAttacking;
    private float magicCooldownTimer;
    private bool isMagicOnCooldown;
    private bool isDead = false;
    private bool isSphereActive = false;

    void Update()
    {
        if (isDead) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsTag("Attack");
        bool isTakingDamage = stateInfo.IsTag("Damage");

        if (isAttacking || isTakingDamage)
        {
            ResetMovement();
            return;
        }

        UpdateMagicCooldown();
        HandleAttacks();
        HandleMovement();
    }

    void ResetMovement()
    {
        controller.Move(Vector3.zero);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetFloat("Speed", 0f);
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

    void HandleAttacks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TriggerMeleeAttack();
        }
        else if (Input.GetMouseButtonDown(1) && !isMagicOnCooldown && !isSphereActive)
        {
            TriggerMagicAttack();
        }
    }

    void TriggerMeleeAttack()
    {
        animator.SetTrigger("Attack");
        controller.Move(Vector3.zero);
    }

    void TriggerMagicAttack()
    {
        animator.SetTrigger("MagicAttack");
        controller.Move(Vector3.zero);
        isMagicOnCooldown = true;
        magicCooldownTimer = magicAttackCooldown;

        StartCoroutine(HandleMagicSphere());
    }

    IEnumerator HandleMagicSphere()
    {
        isSphereActive = true;

        // Сброс позиции и активация сферы
        magicSphere.transform.position = magicSphereStartPos.position;
        magicSphere.SetActive(true);

        // Запуск анимации
        Animator sphereAnimator = magicSphere.GetComponent<Animator>();
        if (sphereAnimator != null)
        {
            sphereAnimator.Play("MagicSphereAnimation", 0, 0f);
        }

        // Ожидание завершения анимации
        yield return new WaitForSeconds(sphereAnimationDuration);

        // Деактивация сферы
        magicSphere.SetActive(false);
        isSphereActive = false;
    }

    // Остальные методы остаются без изменений
    void HandleMovement()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

        UpdateAnimations(direction);

        if (direction.magnitude >= 0.1f)
        {
            RotateCharacter(direction);
            MoveCharacter(direction);
        }
    }

    void UpdateAnimations(Vector3 direction)
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool hasInput = direction.magnitude >= 0.1f;

        animator.SetBool("IsRunning", isRunning && hasInput);
        animator.SetBool("IsWalking", hasInput && !isRunning);
        animator.SetFloat("Speed", hasInput ? (isRunning ? runSpeed : walkSpeed) : 0f);
    }

    void RotateCharacter(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        float smoothedAngle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime
        );
        transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
    }

    void MoveCharacter(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDir.normalized * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) * Time.deltaTime);
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        controller.Move(Vector3.zero);
    }

    public float GetCooldownProgress()
    {
        return isMagicOnCooldown ?
            1 - (magicCooldownTimer / magicAttackCooldown) :
            1f;
    }

    public bool IsMagicReady()
    {
        return !isMagicOnCooldown;
    }
}