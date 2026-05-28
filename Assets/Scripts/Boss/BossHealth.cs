using UnityEngine;
using System.Collections.Generic;


public class BossHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int currentHealth;

    [Header("Imunita")]
    public List<WeaponCharge.Element> immuneElements;

    [Header("References")]
    public BossHealthBar healthBar;

    public System.Action onDeath;
    public System.Action<int, int> onHealthChanged;

    KnockbackReceiver _knockback;
    BossAI _ai;

    void Awake()
    {
        currentHealth = maxHealth;
        _knockback = GetComponent<KnockbackReceiver>();
        _ai = GetComponent<BossAI>();
    }

    void Start()
    {
        healthBar?.Init(maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int dmg, Vector2 knockDir, WeaponCharge.Element element, bool charged)
    {
        if (immuneElements != null && immuneElements.Contains(element))
        {
            Debug.Log($"Boss je imunní na {element}!");
            return;
        }

        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        healthBar?.UpdateBar(currentHealth);

        if (_knockback != null && knockDir != Vector2.zero)
            _knockback.ApplyKnockback(knockDir);

        if (charged)
            _ai?.OnElementHit(element, knockDir);

        if (currentHealth <= 0)
            Die();
    }

    public void TakePureDamage(int dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        healthBar?.UpdateBar(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        onDeath?.Invoke();
        SkillPointManager.Instance?.AddPoint(3);
        Destroy(gameObject);
    }
}
