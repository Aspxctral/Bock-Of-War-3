using UnityEngine;

public class UnarmedHit : MonoBehaviour
{
    [Header("Damage & Shake")]
    public float baseDamage = 15f;
    public float shakeIntensity = 0.12f;
    public float shakeDuration = 0.16f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                // Damage scales with strength
                float damage = baseDamage + PlayerStats.Instance.strength * 0.01f;
                health.TakeDamage(damage, direction);
            }

            if (ScreenShake.Instance != null)
                ScreenShake.Instance.Shake(shakeIntensity, shakeDuration);

            // Increase strength on punch
            PlayerStats.Instance.AddStrength(10);
        }
    }
}