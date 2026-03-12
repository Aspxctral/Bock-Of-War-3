using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 8f;      // ← tweak strength
    public float knockbackDuration = 0.4f; // ← how long enemy is stunned/pushed

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

    // New overload so we can pass knockback direction
    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        currentHealth -= amount;

        if (anim != null)
            anim.SetTrigger("Hit");

        // Knockback
        if (rb != null && hitDirection != Vector3.zero)
        {
            if (agent != null) agent.enabled = false;   // pause AI

            Vector3 force = hitDirection.normalized * knockbackForce;
            rb.AddForce(force, ForceMode.Impulse);

            // Re-enable NavMeshAgent after short delay
            Invoke(nameof(ReEnableAgent), knockbackDuration);
        }

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    private void ReEnableAgent()
    {
        if (agent != null) agent.enabled = true;
    }

    // Old version for safety (if anything calls it without direction)
    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector3.zero);
    }
}