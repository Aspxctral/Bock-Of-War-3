using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;


    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"Player took {amount} damage! Health left: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        // disable player controls
        GetComponent<CharacterController>().enabled = false;

        // optional: disable fighter script
        GetComponent<Fighter>().enabled = false;
    }

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;
}