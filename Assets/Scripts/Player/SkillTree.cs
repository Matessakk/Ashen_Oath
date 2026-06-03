using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance { get; private set; }

    public enum Skill { Fire, Water, Earth, Air, HP }

    [Header("References (Auto-Assigned at Runtime)")]
    public WeaponCharge weaponCharge;
    private PlayerHealth playerHealth;

    [Header("Upgrade hodnoty")]
    public int fireExtraDamage = 1;
    public float waterSlowBonus = 1f;
    public float earthStunBonus = 0.5f;
    public float airKnockbackBonus = 1f;

    public bool fireUnlocked { get; private set; }
    public bool waterUnlocked { get; private set; }
    public bool earthUnlocked { get; private set; }
    public bool airUnlocked { get; private set; }
    public bool hpUnlocked { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keeps your unlocked upgrades intact between levels
    }

    private void Update()
    {
        // Automatically searches for and hooks the player if a scene transition occurred
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
        }
    }

    public bool UnlockFire()
    {
        if (fireUnlocked || !SkillPointManager.Instance.SpendPoint()) return false;
        fireUnlocked = true;
        return true;
    }

    public bool UnlockWater()
    {
        if (waterUnlocked || !SkillPointManager.Instance.SpendPoint()) return false;
        waterUnlocked = true;
        return true;
    }

    public bool UnlockEarth()
    {
        if (earthUnlocked || !SkillPointManager.Instance.SpendPoint()) return false;
        earthUnlocked = true;
        return true;
    }

    public bool UnlockAir()
    {
        if (airUnlocked || !SkillPointManager.Instance.SpendPoint()) return false;
        airUnlocked = true;
        return true;
    }

    public bool UnlockHP()
    {
        if (hpUnlocked || !SkillPointManager.Instance.SpendPoint()) return false;
        hpUnlocked = true;

        if (playerHealth != null)
        {
            playerHealth.maxHealth += 1;
            playerHealth.currentHealth += 1;
            playerHealth.onHealthChanged?.Invoke(playerHealth.currentHealth, playerHealth.maxHealth);
        }
        return true;
    }

    public void ForceUnlock(Skill skill)
    {
        switch (skill)
        {
            case Skill.Fire: fireUnlocked = true; break;
            case Skill.Water: waterUnlocked = true; break;
            case Skill.Earth: earthUnlocked = true; break;
            case Skill.Air: airUnlocked = true; break;
            case Skill.HP:
                hpUnlocked = true;
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 1;
                    playerHealth.onHealthChanged?.Invoke(playerHealth.currentHealth, playerHealth.maxHealth);
                }
                break;
        }
    }

    public int GetFireDamage(int baseDamage) => baseDamage + (fireUnlocked ? fireExtraDamage : 0);
    public float GetSlowDuration(float baseDuration) => baseDuration + (waterUnlocked ? waterSlowBonus : 0f);
    public float GetStunDuration(float baseDuration) => baseDuration + (earthUnlocked ? earthStunBonus : 0f);
    public float GetAirKnockback(float baseForce) => baseForce + (airUnlocked ? airKnockbackBonus : 0f);
}