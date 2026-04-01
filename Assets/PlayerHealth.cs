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
    public Image fillImage;

    [Header("Death Screen")]
    public GameObject deathScreen;

    [Header("Regen")]
    public float regenDelay = 15f;
    public float regenRate = 5f;
    private float lastDamageTime;

    private PlayerMovement movementScript;
    private Fighter fighterScript;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        lastDamageTime = Time.time;

        movementScript = GetComponent<PlayerMovement>();
        fighterScript = GetComponent<Fighter>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (deathScreen != null)
            deathScreen.SetActive(false);

        UpdateHealthUI();
    }

    void Update()
    {
        if (!isDead)
            HandleRegen();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        lastDamageTime = Time.time;

        Debug.Log($"Player took {amount} damage! Health left: {currentHealth}/{maxHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void HandleRegen()
    {
        if (Time.time - lastDamageTime >= regenDelay)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += regenRate * Time.deltaTime;
                currentHealth = Mathf.Min(currentHealth, maxHealth);
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
            float percent = currentHealth / maxHealth;
            if (percent > 0.6f)
                fillImage.color = Color.green;
            else if (percent > 0.3f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }

    // FIXED: OnLevelUp method (called from PlayerStats)
    public void OnLevelUp(int level)
    {
        Debug.Log("LEVEL UP CALLED on PlayerHealth: Level " + level);

        // Increase max health on level up
        maxHealth = 100f + (level - 1) * 10f;
        currentHealth = maxHealth;   // Fully heal on level up

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        UpdateHealthUI();
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
            capsule.enabled = false;

        if (deathScreen != null)
            deathScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}