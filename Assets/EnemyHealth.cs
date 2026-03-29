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

    [Header("Rewards")]
    public int xpReward = 25;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (anim != null)
            anim.SetTrigger("Hit");

        // Knockback
        if (rb != null && hitDirection != Vector3.zero)
        {
            if (agent != null) agent.enabled = false;
            rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode.Impulse);
            Invoke(nameof(ReEnableAgent), knockbackDuration);
        }

        if (currentHealth <= 0)
            Die();
    }

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
        if (agent != null) agent.enabled = true;
    }
}