using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float MaxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI")]
    public Slider healthBar;
    public Image fillImage;

    [Header("Regen")]
    public float regenDelay = 15f; // seconds before regen starts
    public float regenRate = 5f;   // health per second
    private float lastDamageTime;

    // References
    private PlayerMovement movementScript;
    private Fighter fighterScript;
    private Animator animator;

    void Start()
    {
        currentHealth = MaxHealth;
        lastDamageTime = Time.time;

        movementScript = GetComponent<PlayerMovement>();
        fighterScript = GetComponent<Fighter>();

        if (healthBar != null)
        {
            healthBar.maxValue = MaxHealth;
            healthBar.value = currentHealth;
        }

        UpdateHealthUI();
    }

    void Update()
    {
        HandleRegen();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        lastDamageTime = Time.time; // 🔥 reset regen timer

        Debug.Log($"Player took {amount} damage! Health left: {currentHealth}/{MaxHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void HandleRegen()
    {
        if (isDead) return;

        // Check if enough time has passed since last damage
        if (Time.time - lastDamageTime >= regenDelay)
        {
            if (currentHealth < MaxHealth)
            {
                currentHealth += regenRate * Time.deltaTime;
                currentHealth = Mathf.Min(currentHealth, MaxHealth);

                UpdateHealthUI();
            }
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (fillImage != null)
        {
            float healthPercent = currentHealth / MaxHealth;

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

        Destroy(gameObject);
    }

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;
}