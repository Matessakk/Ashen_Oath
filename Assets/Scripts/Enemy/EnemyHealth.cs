using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    private KnockbackReceiver knockback;
    private EnemyAudio enemyAudio;

    private void Awake()
    {
        currentHealth = maxHealth;
        knockback = GetComponent<KnockbackReceiver>();
        enemyAudio = GetComponent<EnemyAudio>();
    }

    public void TakeDamage(int dmg, Vector2 knockDir)
    {
        currentHealth -= dmg;

        if (enemyAudio != null)
            enemyAudio.PlayHitSound();

        if (knockback != null)
            knockback.ApplyKnockback(knockDir);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        
        Destroy(gameObject);
        
    }
}



