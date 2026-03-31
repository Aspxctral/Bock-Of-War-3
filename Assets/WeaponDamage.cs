using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int baseDamage = 25;
    public Collider damageCollider;

    [Header("Screen Shake")]
    public float shakeIntensity = 0.25f;
    public float shakeDuration = 0.22f;

    private bool canDealDamage = false;

    void Start()
    {
        if (damageCollider != null) damageCollider.enabled = false;
    }

    public void EnableDamage()
    {
        canDealDamage = true;
        if (damageCollider != null) damageCollider.enabled = true;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        if (damageCollider != null) damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        if (other.CompareTag("Enemy"))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // Damage scales with strength
                int damage = baseDamage + Mathf.RoundToInt(PlayerStats.Instance.strength * 0.02f);
                enemy.TakeDamage(damage, direction);

                if (ScreenShake.Instance != null)
                    ScreenShake.Instance.Shake(shakeIntensity, shakeDuration);
            }

            // Increase strength on axe hit
            PlayerStats.Instance.AddStrength(20);
        }
    }
}