using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 20;
    private Vector3 flightDirection;
    private float lifetime = 3f;
    private PlayerHealth playerHealth;
    private Element element;

    public void Setup(Element element, int damage, Vector3 direction)
    {
        this.element = element;
        this.damage = damage;
        this.flightDirection = direction;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        if (!GetComponent<Collider>())
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (playerHealth == null || playerHealth.GetCurrentHP() <= 0)
        {
            Destroy(gameObject);
            return;
        }
        transform.position += flightDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                ApplyElementEffect(playerHealth);
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }

    private void ApplyElementEffect(PlayerHealth player)
    {
        // Здесь можно реализовать уникальные эффекты для каждой стихии
        switch (element)
        {
            case Element.Fire:
                // player.ApplyBurnEffect(3f, damage/4);
                break;
            case Element.Ice:
                // player.ApplySlowEffect(2f, 0.5f);
                break;
            case Element.Earth:
                // player.ApplyStunEffect(1f);
                break;
            case Element.Ether:
                // player.ApplyConfusionEffect(2f);
                break;
        }
    }
}