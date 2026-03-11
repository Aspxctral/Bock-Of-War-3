using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float damage = 25f;

    private bool canDealDamage = false;

    public void EnableDamage()
    {
        canDealDamage = true;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}