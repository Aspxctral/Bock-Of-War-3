using UnityEngine;

public class EnemyKickDamage : MonoBehaviour
{
    public float damage = 20f;
    public float attackCooldown = 1.2f;

    private bool canDealDamage = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);

            canDealDamage = false;
            Invoke(nameof(ResetDamage), attackCooldown);
        }
        Debug.Log("Enemy collider hit: " + other.name);
    }

    void ResetDamage()
    {
        canDealDamage = true;
    }
}