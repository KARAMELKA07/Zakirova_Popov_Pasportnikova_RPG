using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 20;
    public Element element;

    [Header("Effects")]
    public ParticleSystem trailEffect;
    public AudioClip impactSound;

    private Vector3 direction;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, lifetime);
    }

    // Новый метод для настройки снаряда
    public void Setup(Element elementType, int damageAmount, Vector3 moveDirection)
    {
        element = elementType;
        damage = damageAmount;
        direction = moveDirection.normalized;
        ConfigureAppearance();
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void ConfigureAppearance()
    {
        if (trailEffect != null)
        {
            var main = trailEffect.main;
            switch (element)
            {
                case Element.Fire:
                    main.startColor = new Color(1f, 0.3f, 0f);
                    break;
                case Element.Ice:
                    main.startColor = new Color(0.3f, 0.8f, 1f);
                    break;
                case Element.Earth:
                    main.startColor = new Color(0.5f, 0.3f, 0.1f);
                    break;
                case Element.Ether:
                    main.startColor = new Color(0.7f, 0.1f, 1f);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                if (impactSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(impactSound);
                }
            }
        }
        Destroy(gameObject);
    }
}