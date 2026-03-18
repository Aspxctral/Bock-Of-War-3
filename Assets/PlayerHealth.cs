using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI")]
    public Slider healthBar;
    public Image fillImage; // assign the Fill image of the slider

    // References
    private PlayerMovement movementScript;
    private Fighter fighterScript;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        movementScript = GetComponent<PlayerMovement>();
        fighterScript = GetComponent<Fighter>();
        animator = GetComponent<Animator>();

        // Setup UI
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"Player took {amount} damage! Health left: {currentHealth}/{maxHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (fillImage != null)
        {
            float healthPercent = currentHealth / maxHealth;

            if (healthPercent > 0.6f)
                fillImage.color = Color.green;
            else if (healthPercent > 0.3f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        if (movementScript != null)
            movementScript.enabled = false;

        if (fighterScript != null)
            fighterScript.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            capsule.enabled = false;
        }
    }

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;
}