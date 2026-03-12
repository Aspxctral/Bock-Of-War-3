using UnityEngine;

public class UnarmedHit : MonoBehaviour
{
    [Header("Damage & Shake")]
    public float damage = 15f;
    public float shakeIntensity = 0.12f;
    public float shakeDuration = 0.16f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Calculate direction from player → enemy
            Vector3 direction = (other.transform.position - transform.position).normalized;

            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage, direction);
            }

            if (ScreenShake.Instance != null)
                ScreenShake.Instance.Shake(shakeIntensity, shakeDuration);
        }
    }
}