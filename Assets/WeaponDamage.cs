using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 25;
    public Collider damageCollider; // assign your axe collider here

    [Header("Screen Shake on Hit")]
    [Tooltip("How strong the camera shakes when the axe hits an enemy")]
    public float shakeIntensity = 0.25f;
    [Tooltip("How long the shake lasts")]
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
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);

                // ────────────────────────────────
                // SCREEN SHAKE ONLY ON SUCCESSFUL HIT
                if (ScreenShake.Instance != null)
                {
                    ScreenShake.Instance.Shake(shakeIntensity, shakeDuration);
                }
                // ────────────────────────────────
            }
        }
    }
}