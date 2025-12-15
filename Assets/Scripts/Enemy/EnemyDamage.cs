using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockForce = 6f;
    [SerializeField] private float damageCooldown = 0.8f;

    private float nextDamageTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Time.time < nextDamageTime) return;

        PlayerHealth ph = collision.GetComponent<PlayerHealth>();
        KnockbackReceiver kb = collision.GetComponent<KnockbackReceiver>();

        Vector2 dir = (collision.transform.position - transform.position).normalized;

        if (ph != null)
            ph.TakeDamage(damage, dir); 

        if (kb != null)
            kb.ApplyKnockback(dir * knockForce);

        nextDamageTime = Time.time + damageCooldown;
    }
}