using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int baseDamage = 1;
    public int chargedDamage = 5;
    public WeaponCharge weaponCharge;

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyHealth enemy = col.GetComponent<EnemyHealth>();
        if (!enemy) return;

        int dmg = baseDamage;

        if (weaponCharge.TakeCharge())
            dmg = chargedDamage;

        enemy.TakeDamage(dmg, Vector2.zero);
    }
}
