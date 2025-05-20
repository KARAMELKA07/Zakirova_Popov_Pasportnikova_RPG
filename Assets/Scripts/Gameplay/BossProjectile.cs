using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;

    private int damage;
    private Element element;
    private Vector3 direction;
    private PlayerHealth playerHealth;

    void Start()
    {
        Destroy(gameObject, lifetime);
        playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
    }

    public void Setup(Element elementType, int projectileDamage, Vector3 moveDirection)
    {
        element = elementType;
        damage = projectileDamage;
        direction = moveDirection;

        // Здесь можно добавить визуальные эффекты в зависимости от стихии
        ApplyElementVisuals();
    }

    void Update()
    {
        if (playerHealth == null || playerHealth.GetCurrentHP() <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                ApplyElementEffect(); // Применяем эффект стихии
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void ApplyElementVisuals()
    {
        // Здесь можно изменить цвет/эффекты снаряда в зависимости от стихии
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            switch (element)
            {
                case Element.Fire:
                    main.startColor = new Color(1f, 0.3f, 0f);
                    break;
                case Element.Ice:
                    main.startColor = new Color(0.3f, 0.7f, 1f);
                    break;
                case Element.Earth:
                    main.startColor = new Color(0.5f, 0.3f, 0.1f);
                    break;
                case Element.Ether:
                    main.startColor = new Color(0.8f, 0.1f, 0.8f);
                    break;
            }
        }
    }

    private void ApplyElementEffect()
    {
        // Здесь можно добавить различные эффекты для разных стихий
        switch (element)
        {
            case Element.Fire:
                // Эффект горения
                break;
            case Element.Ice:
                // Эффект замедления
                break;
            case Element.Earth:
                // Эффект оглушения
                break;
            case Element.Ether:
                // Эффект магического урона
                break;
        }
    }
}