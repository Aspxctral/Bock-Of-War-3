using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.4f;

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

    // Overload with knockback direction (called by WeaponDamage and UnarmedHit)
    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        currentHealth -= amount;

        if (anim != null)
            anim.SetTrigger("Hit");

        // Knockback
        if (rb != null && hitDirection != Vector3.zero)
        {
            if (agent != null) agent.enabled = false;

            Vector3 force = hitDirection.normalized * knockbackForce;
            rb.AddForce(force, ForceMode.Impulse);

            Invoke(nameof(ReEnableAgent), knockbackDuration);
        }

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    // Original overload (takes only amount) - for safety/compatibility
    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector3.zero); // No knockback if direction not provided
    }

    private void ReEnableAgent()
    {
        if (agent != null) agent.enabled = true;
    }
}