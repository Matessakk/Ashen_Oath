using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    KnockbackReceiver knockback;
    EnemyAudio enemyAudio;
    EnemyStatusEffects statusEffects;

    void Awake()
    {
        currentHealth = maxHealth;
        knockback = GetComponent<KnockbackReceiver>();
        enemyAudio = GetComponent<EnemyAudio>();
        statusEffects = GetComponent<EnemyStatusEffects>();
    }

    public void TakeDamage(int dmg, Vector2 knockDir, WeaponCharge.Element element)
    {
        currentHealth -= dmg;

        if (enemyAudio != null)
            enemyAudio.PlayHitSound();

        if (knockback != null && knockDir != Vector2.zero)
            knockback.ApplyKnockback(knockDir);

        if (statusEffects != null)
        {
            switch (element)
            {
                case WeaponCharge.Element.Fire:
                    statusEffects.ApplyFire();
                    break;

                case WeaponCharge.Element.Water:
                    statusEffects.ApplyWater();
                    break;

                case WeaponCharge.Element.Earth:
                    statusEffects.ApplyEarth();
                    break;

                case WeaponCharge.Element.Air:
                    statusEffects.ApplyAir(knockDir);
                    break;
            }
        }

        if (currentHealth <= 0)
            Die();
    }

    public void TakePureDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}





