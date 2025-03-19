using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 15;
    private Vector3 flightDirection; 
    private float lifetime = 3f; 
    private PlayerHealth playerHealth;

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

    public void SetDirection(Vector3 targetPosition)
    {
        flightDirection = (targetPosition - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(flightDirection);
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
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (!other.CompareTag("Enemy")) 
        {
            Destroy(gameObject);
        }
    }
}