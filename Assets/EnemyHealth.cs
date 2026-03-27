using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.4f;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private Image healthFill;
    private Transform healthBarInstance;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private Animator anim;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

void Start()
{
    if (healthBarPrefab != null)
    {
        healthBarInstance = Instantiate(
            healthBarPrefab,
            transform.position + Vector3.up * 1.8f,
            Quaternion.identity,
            transform
        ).transform;

        // 🔥 safer way to find the fill
        Image[] images = healthBarInstance.GetComponentsInChildren<Image>();

        if (images.Length > 1)
        {
            healthFill = images[1]; // assumes 2nd image = fill
        }
        else
        {
            Debug.LogWarning("Health bar fill not found!");
        }
    }
}

    void Update()
    {
        // Keep bar above enemy + face camera
        if (healthBarInstance != null)
        {
            healthBarInstance.position = transform.position + Vector3.up * 2f;

            if (Camera.main != null)
                healthBarInstance.LookAt(Camera.main.transform);
        }
    }

    // 🔥 Damage WITH knockback
    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        currentHealth -= amount;

        if (anim != null)
            anim.SetTrigger("Hit");

        // Update health bar
        if (healthFill != null)
            healthFill.fillAmount = currentHealth / maxHealth;

        // Knockback
        if (rb != null && hitDirection != Vector3.zero)
        {
            if (agent != null) agent.enabled = false;

            Vector3 force = hitDirection.normalized * knockbackForce;
            rb.AddForce(force, ForceMode.Impulse);

            Invoke(nameof(ReEnableAgent), knockbackDuration);
        }

        if (currentHealth <= 0)
            Die();
    }

    // 🔥 Damage without direction
    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector3.zero);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void ReEnableAgent()
    {
        if (agent != null)
            agent.enabled = true;
    }
}