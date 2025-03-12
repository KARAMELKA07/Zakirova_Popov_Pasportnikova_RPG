using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    private float currentSpeed;
    private CharacterController characterController;
    private Animator animator;
    public Image magicCooldownImage;
    public float magicCooldown = 10f;
    private float magicCooldownTimer = 10f;

    public float turnSpeed = 100f; // Скорость поворота персонажа (градусы в секунду)

    // Таймеры для анимаций атаки
    private float attackTimer = 0f;
    private float magicAttackTimer = 0f;
    public float attackDuration = 1f; // Длительность анимации атаки мечом
    public float magicAttackDuration = 1.5f; // Длительность анимации магической атаки

    // Таймер для задержки после атаки
    private float postAttackCooldown = 0f;
    public float postAttackDelay = 2f; // Задержка после атаки (2 секунды)

    // Ссылка на шар (дочерний объект)
    public GameObject magicSphere; // Перетащите сюда ваш шар из инспектора

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // Подключаем Animator

        // Убедимся, что шар изначально неактивен
        if (magicSphere != null)
        {
            magicSphere.SetActive(false);
        }
    }

    void Update()
    {
        HandleMagicCooldown();
        HandleAttacks();
        UpdateAttackTimers();
        UpdatePostAttackCooldown();

        if (!IsAttacking() && postAttackCooldown <= 0)
        {
            MovePlayer();
        }
        else
        {
            ForceIdle();
        }
        if (Input.GetMouseButtonDown(1) && magicCooldownTimer <= 0)
        {
            CastMagic();
        }
    }

    void HandleMagicCooldown()
    {
        if (magicCooldownTimer > 0)
        {
            magicCooldownTimer -= Time.deltaTime;
            magicCooldownImage.fillAmount = 1 - (magicCooldownTimer / magicCooldown);
        }
    }

// Исправленный метод магической атаки
    void CastMagic()
    {
        if (magicCooldownTimer > 0) return; // Блокируем каст, если кулдаун еще идет

        animator.SetBool("IsMagicAttacking", true);
        magicCooldownTimer = magicCooldown; // Устанавливаем кулдаун
        magicCooldownImage.fillAmount = 0; // Обновляем индикатор
    }
    void FinishMagicAttack()
    {
        animator.SetBool("IsMagicAttacking", false);
    }

    void MovePlayer()
    {
        // Получаем входные данные для движения
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Направление движения (пока что только вперед или назад)
        Vector3 moveDirection = transform.forward * moveZ;

        // Проверка, движется ли персонаж
        bool isMoving = moveDirection.magnitude > 0.1f;

        // Проверяем, удерживается ли Shift для бега
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Поворот персонажа на месте
        if (moveX > 0) // Двигаемся вправо
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime); // Поворот вправо
        }
        else if (moveX < 0) // Двигаемся влево
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime); // Поворот влево
        }

        // Двигаем персонажа по направлению, куда он смотрит
        if (isMoving)
        {
            characterController.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
        }

        // Передаем информацию в Animator
        animator.SetFloat("Speed", isMoving ? currentSpeed : 0); // Устанавливаем скорость

        // Устанавливаем параметры для анимации
        if (isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift)) // Если зажат Shift, то бегаем
            {
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsWalking", false); // Останавливаем ходьбу
            }
            else // Если Shift не зажат, то просто идем
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", true); // Включаем ходьбу
            }
        }
        else
        {
            animator.SetBool("IsRunning", false); // Останавливаем бег
            animator.SetBool("IsWalking", false); // Останавливаем ходьбу
        }
    }

    void HandleAttacks()
    {
        // Левая кнопка мыши - атака мечом
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0) // 0 - левая кнопка мыши
        {
            animator.SetBool("IsAttacking", true);
            attackTimer = attackDuration; // Запускаем таймер атаки
        }

        // Правая кнопка мыши - магическая атака
        if (Input.GetMouseButtonDown(1) && magicAttackTimer <= 0) // 1 - правая кнопка мыши
        {
            animator.SetBool("IsMagicAttacking", true);
            magicAttackTimer = magicAttackDuration; // Запускаем таймер магической атаки

            // Активируем шар и запускаем его анимацию
            if (magicSphere != null)
            {
                magicSphere.SetActive(true); // Активируем шар
                Animation sphereAnimation = magicSphere.GetComponent<Animation>();
                if (sphereAnimation != null)
                {
                    sphereAnimation.Play(); // Запускаем анимацию шара
                }
            }
        }
    }

    void UpdateAttackTimers()
    {
        // Обновляем таймер атаки мечом
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                animator.SetBool("IsAttacking", false); // Сбрасываем атаку
                postAttackCooldown = postAttackDelay; // Запускаем задержку после атаки
            }
        }

        // Обновляем таймер магической атаки
        if (magicAttackTimer > 0)
        {
            magicAttackTimer -= Time.deltaTime;
            if (magicAttackTimer <= 0)
            {
                animator.SetBool("IsMagicAttacking", false); // Сбрасываем магическую атаку
                postAttackCooldown = postAttackDelay; // Запускаем задержку после атаки

                // Деактивируем шар после завершения анимации
                if (magicSphere != null)
                {
                    magicSphere.SetActive(false);
                }
            }
        }
    }

    void UpdatePostAttackCooldown()
    {
        // Обновляем таймер задержки после атаки
        if (postAttackCooldown > 0)
        {
            postAttackCooldown -= Time.deltaTime;
        }
    }

    // Принудительно включаем анимацию idle
    void ForceIdle()
    {
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", false);
        animator.SetFloat("Speed", 0); // Устанавливаем скорость в 0
    }

    // Проверка, атакует ли персонаж
    bool IsAttacking()
    {
        return animator.GetBool("IsAttacking") || animator.GetBool("IsMagicAttacking") || magicCooldownTimer > 0;
    }

}