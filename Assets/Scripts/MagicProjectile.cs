using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 15;
    private Vector3 flightDirection; // Направление полета
    private float lifetime = 3f; // Время жизни снаряда

    void Start()
    {
        // Уничтожаем объект через заданное время
        Destroy(gameObject, lifetime);

        // Добавляем коллайдер (если еще не добавлен)
        if (!GetComponent<Collider>())
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }
    }

    public void SetDirection(Vector3 targetPosition)
    {
        // Рассчитываем начальное направление
        flightDirection = (targetPosition - transform.position).normalized;

        // Поворачиваем снаряд в направлении полета
        transform.rotation = Quaternion.LookRotation(flightDirection);
    }

    void Update()
    {
        // Движение в заданном направлении
        transform.position += flightDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем столкновение с игроком
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // Дополнительная логика при столкновении с другими объектами
        if (!other.CompareTag("Enemy")) // Не уничтожаем при столкновении с врагами
        {
            Destroy(gameObject);
        }
    }
}