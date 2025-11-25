using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("iFrames")]
    public float invincibilityTime = 0.3f;
    private bool isInvincible;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Player HP: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(int amount, Vector2 knockDirection)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log("Player HP: " + currentHealth);

        
        GetComponent<KnockbackReceiver>()?.ApplyKnockback(knockDirection);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Invincibility());
    }

    private System.Collections.IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("PLAYER DIED ");
        // sem pak dáš animaci / respawn / game over
    }
}
